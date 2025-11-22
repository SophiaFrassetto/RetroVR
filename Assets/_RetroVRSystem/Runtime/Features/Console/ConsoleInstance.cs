using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using SK.Libretro.Unity;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    [RequireComponent(typeof(LibretroInstance)), RequireComponent(typeof(AudioSource))]
    public class ConsoleInstance : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Console Configuration")]
        [SerializeField] private ConsoleDefinition consoleDefinition;
        [SerializeField] private TMP_Text displayConsoleName;
        [SerializeField] private bool playAudio = true;
        #endregion

        #region State Handling
        [Header("Console State")]

        [SerializeField]
        private ConsoleOperationalState operationalState = ConsoleOperationalState.Off;

        [SerializeField]
        private ConsolePhysicalState physicalState = ConsolePhysicalState.Loose;
        public event Action<ConsoleOperationalState> OnOperationalStateChanged;
        public event Action<ConsolePhysicalState> OnPhysicalStateChanged;
        #endregion

        #region Component References
        [Header("Libretro Instance")]
        private LibretroInstanceVariable emulatorInstance;

        [Header("Screen Configuration")]
        [SerializeField] private ScreenInstance screenInstance;

        [Header("Cartridge Configuration")]
        private CartridgeInstance insertedCartridge;

        // Optional audio feedback (serialize if you want sounds; keep null-safe)
        [Header("Optional Feedback (audio)")]
        [SerializeField] private AudioSource feedbackAudioSource;
        [SerializeField] private AudioClip clipPowerOn;
        [SerializeField] private AudioClip clipInsertCartridge;
        [SerializeField] private AudioClip clipPowerOff;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (consoleDefinition != null && displayConsoleName != null)
            {
                displayConsoleName.text = consoleDefinition.consoleName;
            }

            if (emulatorInstance == null)
            {
                emulatorInstance = ScriptableObject.CreateInstance<LibretroInstanceVariable>();
            }

            emulatorInstance.Current = GetComponent<LibretroInstance>();
            insertedCartridge = null;

            SetOperationalState(ConsoleOperationalState.Off);
            SetPhysicalState(ConsolePhysicalState.Loose);
        }
        #endregion

        #region Console Handling
        private bool GetRunningState()
        {
            return emulatorInstance.Current.Running;
        }

        private bool CanAcceptCartridge(CartridgeDefinition cartridgeDefinition)
        {
            if (!consoleDefinition.supportedExtensions.Contains(cartridgeDefinition.extension))
            {
                Log.Error($"[ConsoleInstance] Cartridge extension '{cartridgeDefinition.extension}' is not supported by {consoleDefinition.consoleName}.");
                return false;
            }

            return true;
        }

        private string CoreToUse()
        {
            return string.IsNullOrEmpty(insertedCartridge.cartridgeDefinition.overrideCoreName)
            ? consoleDefinition.coreName
            : insertedCartridge.cartridgeDefinition.overrideCoreName;
        }
        #endregion

        #region Power Handling
        public void PowerOn()
        {
            TryPlayOneShot(clipPowerOn);

            if (consoleDefinition == null)
            {
                Log.Error("[ConsoleInstance] Console definition is not set.");
                return;
            }

            if (insertedCartridge == null)
            {
                Log.Warn("[ConsoleInstance] PowerOn requested but no cartridge present. Use Power() to enter Standby.");
                // set to Standby to reflect "powered but waiting"
                SetOperationalState(ConsoleOperationalState.Standby);
                return;
            }

            // if emulator already running, ignore
            if (operationalState == ConsoleOperationalState.Running)
            {
                Log.Warn("[ConsoleInstance] PowerOn requested but emulator is already running.");
                return;
            }

            SetOperationalState(ConsoleOperationalState.Initializing);

            // make sure emulator is initialized with the current cartridge and screen
            InitializeEmulator();

            // start the emulation runtime
            emulatorInstance.StartContent();

            // wait for libretro to report running
            StartCoroutine(WaitForEmulatorRunningCoroutine(5f));
        }

        public void PowerOff()
        {
            TryPlayOneShot(clipPowerOff);
            bool wasPowered = IsPowered();

            // If already off, nothing to do
            if (!wasPowered)
            {
                Log.Warn("[ConsoleInstance] PowerOff requested but console is already Off or in Error.");
                SetOperationalState(ConsoleOperationalState.Off);
                return;
            }

            if (operationalState == ConsoleOperationalState.Running)
            {
                SetOperationalState(ConsoleOperationalState.ShuttingDown);
            }

            bool instanceCurrentExists = emulatorInstance != null && emulatorInstance.Current != null;

            // If emulator is running, stop it gracefully
            if (operationalState == ConsoleOperationalState.ShuttingDown && instanceCurrentExists && emulatorInstance.Current.Running)
            {
                try
                {
                    emulatorInstance.StopContent();
                    // deinitialize the emulator as we are powering off (safe-guard)
                    DeInitializeEmulator();
                }
                catch (Exception ex)
                {
                    Log.Error($"[ConsoleInstance] Error while stopping emulator: {ex.Message}");
                }
            }
            else
            {
                // Not running (Standby or CartridgeInserted), ensure emulator is deinitialized
                if (instanceCurrentExists)
                {
                    DeInitializeEmulator();
                }
            }

            // Final state is Off
            StartCoroutine(WaitForEmulatorOffCoroutine(5f));
        }

        public void ResetConsole()
        {
            if (operationalState != ConsoleOperationalState.Running || !emulatorInstance.Current.Running)
            {
                Log.Error("[ConsoleInstance] Console is not powered on or emulator is not running.");
                return;
            }
            PowerOff();
            PowerOn();
        }
        #endregion

        #region Cartridge Handling
        public void InsertCartridge(SelectEnterEventArgs args)
        {
            // Do not allow insert if we already have a cartridge
            if (insertedCartridge != null)
            {
                Log.Warn("[ConsoleInstance] A cartridge is already inserted. Eject before inserting a new one.");
                return;
            }

            // capture powered state BEFORE we change any operational state
            bool wasPowered = IsPowered();

            var interactorable = args.interactableObject;
            if (interactorable == null)
            {
                Log.Error("[ConsoleInstance] InsertCartridge called with null interactable.");
                return;
            }

            var cartridgeObject = interactorable.transform.gameObject;
            CartridgeInstance cartridgeInstance = cartridgeObject.GetComponent<CartridgeInstance>();
            if (cartridgeInstance == null)
            {
                Log.Error("[ConsoleInstance] InsertCartridge: object is not a CartridgeInstance.");
                return;
            }

            if (cartridgeInstance.cartridgeDefinition == null)
            {
                Log.Error("[ConsoleInstance] Cannot insert a null cartridge definition.");
                return;
            }

            if (!CanAcceptCartridge(cartridgeInstance.cartridgeDefinition))
            {
                Log.Error("[ConsoleInstance] Cartridge rejected by CanAcceptCartridge.");
                return;
            }

            // Accept cartridge into slot (physical + logical)
            insertedCartridge = cartridgeInstance;
            SetPhysicalState(ConsolePhysicalState.Placed);
            SetOperationalState(ConsoleOperationalState.CartridgeInserted);
            TryPlayOneShot(clipInsertCartridge);

            Log.Info($"[ConsoleInstance] Cartridge '{insertedCartridge.cartridgeDefinition.romName}' inserted.");

            // Use the value captured BEFORE we set CartridgeInserted
            if (wasPowered)
            {
                // If already running ignore; otherwise initialize/start
                if (operationalState != ConsoleOperationalState.Running)
                {
                    SetOperationalState(ConsoleOperationalState.Initializing);

                    // attempt initialization; only start content if initialization succeeded
                    if (InitializeEmulator())
                    {
                        emulatorInstance.StartContent();
                        StartCoroutine(WaitForEmulatorRunningCoroutine(5f));
                    }
                    else
                    {
                        Log.Error("[ConsoleInstance] Initialization failed: preconditions not met (emulator/def/cart missing).");
                        SetOperationalState(ConsoleOperationalState.Error);
                    }
                }
            }
        }

        public void EjectCartridge()
        {
            if (insertedCartridge == null)
            {
                Log.Warn("[ConsoleInstance] No cartridge to eject.");
                return;
            }

            // If emulator is running, require explicit PowerOff before ejecting
            if (operationalState == ConsoleOperationalState.Running)
            {
                Log.Warn("[ConsoleInstance] Cannot eject cartridge while emulator is running. Please PowerOff first.");
                return;
            }

            // safe to deinitialize and remove cartridge
            try
            {
                if (emulatorInstance?.Current != null)
                {
                    emulatorInstance.Current.DeInitialize();
                    emulatorInstance.Current.Renderer = null;
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"[ConsoleInstance] Swallowed exception during DeInitialize: {ex.Message}");
            }

            // clear cartridge
            var romName = insertedCartridge.cartridgeDefinition?.romName;
            insertedCartridge = null;

            // if console still powered (e.g., was Standby) remain in Standby; otherwise Off
            if (IsPowered())
            {
                SetOperationalState(ConsoleOperationalState.Standby);
            }
            else
            {
                SetOperationalState(ConsoleOperationalState.Off);
            }

            SetPhysicalState(ConsolePhysicalState.Loose);

            Log.Info($"[ConsoleInstance] Cartridge '{romName}' ejected.");
        }
        #endregion

        #region Emulator Handling
        private bool InitializeEmulator()
        {
            if (emulatorInstance == null || consoleDefinition == null || insertedCartridge == null)
            {
                Log.Error("[ConsoleInstance] InitializeEmulator: preconditions not met (emulator/def/cart missing).");
                return false;
            }

            emulatorInstance.Current.Initialize(CoreToUse(), insertedCartridge.cartridgeDefinition.romsDirectory, insertedCartridge.cartridgeDefinition.romName);
            emulatorInstance.Current.Renderer = screenInstance?.screenRenderer;
            emulatorInstance.Current.Collider = screenInstance?.screenCollider;
            return true;
        }

        private void DeInitializeEmulator()
        {
            if (emulatorInstance == null)
            {
                Log.Error("[ConsoleInstance] Emulator instance is not set.");
                return;
            }

            emulatorInstance.Current.DeInitialize();
            emulatorInstance.Current.Renderer = null;
        }
        #endregion

        #region Internal Helpers
        /// <summary>
        /// Change the operational state of this console and fire events.
        /// Keep local reactions lightweight; heavy VFX / audio should live in a manager.
        /// </summary>
        public void SetOperationalState(ConsoleOperationalState newState)
        {
            if (operationalState == newState) return;

            operationalState = newState;
            Log.Info($"[ConsoleInstance] OperationalState -> {operationalState}");
            OnOperationalStateChanged?.Invoke(operationalState);

            // Local minimal reactions (non-blocking)
            switch (operationalState)
            {
                case ConsoleOperationalState.Initializing:
                    // optionally notify screen to enter Booting (via screenInstance)
                    if (screenInstance != null)
                        Log.Info("[ConsoleInstance] Screen would enter Booting state here.");
                        // screenInstance.SetOperationalState(ScreenOperationalState.Booting);
                    break;

                case ConsoleOperationalState.Running:
                    if (screenInstance != null)
                        Log.Info("[ConsoleInstance] Screen would enter ShowingContent state here.");
                        // screenInstance.SetOperationalState(ScreenOperationalState.ShowingContent);
                    break;

                case ConsoleOperationalState.ShuttingDown:
                    // We could animate shutdown visual here
                    break;

                case ConsoleOperationalState.Off:
                    if (screenInstance != null)
                        Log.Info("[ConsoleInstance] Screen would enter Off state here.");
                        // screenInstance.SetOperationalState(ScreenOperationalState.Off);
                    break;

                case ConsoleOperationalState.Error:
                    if (screenInstance != null)
                        Log.Info("[ConsoleInstance] Screen would enter Error state here.");
                        // screenInstance.SetOperationalState(ScreenOperationalState.Error);
                    break;
            }
        }

        /// <summary>
        /// Change the physical state (Held, Placed, Connected) of the console.
        /// </summary>
        public void SetPhysicalState(ConsolePhysicalState newState)
        {
            if (physicalState == newState) return;
            physicalState = newState;
            Log.Info($"[ConsoleInstance] PhysicalState -> {physicalState}");
            OnPhysicalStateChanged?.Invoke(physicalState);

            // Local reactions (snap transforms, lock rigidbody, etc) - keep small here
            switch (physicalState)
            {
                case ConsolePhysicalState.Held:
                    // e.g., disable rigidbody gravity if needed
                    break;
                case ConsolePhysicalState.Placed:
                    // e.g., re-enable physics or fix rotation
                    break;
                case ConsolePhysicalState.ConnectedToScreen:
                    // ensure emulator renderer is assigned
                    if (emulatorInstance?.Current != null && screenInstance != null)
                    {
                        emulatorInstance.Current.Renderer = screenInstance.screenRenderer;
                        emulatorInstance.Current.Collider = screenInstance.screenCollider;
                    }
                    break;
            }
        }

        /// <summary>
        /// Play a one-shot audio clip if available.
        /// </summary>
        private void TryPlayOneShot(AudioClip clip)
        {
            if (!playAudio) return;
            if (clip == null || feedbackAudioSource == null) return;
            feedbackAudioSource.PlayOneShot(clip);

        }

        /// <summary>
        /// Waits until the Libretro instance reports Running or times out.
        /// </summary>
        private IEnumerator WaitForEmulatorRunningCoroutine(float timeoutSeconds = 5f)
        {
            float t = 0f;
            while (t < timeoutSeconds)
            {
                if (emulatorInstance != null && emulatorInstance.Current != null && emulatorInstance.Current.Running)
                {
                    SetOperationalState(ConsoleOperationalState.Running);
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

            // timed out
            Log.Warn("[ConsoleInstance] Timeout waiting for emulator to report Running.");
            SetOperationalState(ConsoleOperationalState.Error);
        }

        /// <summary>
        /// Waits until the Libretro instance reports Off and deinitialized or times out.
        /// </summary>
        private IEnumerator WaitForEmulatorOffCoroutine(float timeoutSeconds = 5f)
        {
            float t = 0f;
            while (t < timeoutSeconds)
            {
                if (emulatorInstance != null && emulatorInstance.Current != null && !emulatorInstance.Current.Running)
                {
                    DeInitializeEmulator();
                    SetOperationalState(ConsoleOperationalState.Off);
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

            // timed out
            Log.Warn("[ConsoleInstance] Timeout waiting for emulator to report Off.");
            SetOperationalState(ConsoleOperationalState.Error);
        }

        /// <summary>
        /// Returns true if the console is considered powered (not Off and not Error).
        /// This is a lightweight "powered" concept separate from emulator running.
        /// </summary>
        private bool IsPowered()
        {
            return operationalState != ConsoleOperationalState.Off &&
                operationalState != ConsoleOperationalState.Error &&
                operationalState != ConsoleOperationalState.ShuttingDown;
        }

        /// <summary>
        /// Convenience wrapper for push-style buttons.
        /// If console is powered -> PowerOff(); otherwise -> PowerOn() (or Standby if no cartridge).
        /// </summary>
        public void Power()
        {
            // If currently in the middle of starting/shutting it's safer to ignore toggle requests
            if (operationalState == ConsoleOperationalState.Initializing || operationalState == ConsoleOperationalState.ShuttingDown)
            {
                Log.Warn("[ConsoleInstance] Power toggle ignored while transitioning.");
                return;
            }

            bool wasPowered = IsPowered();

            if (wasPowered && operationalState != ConsoleOperationalState.CartridgeInserted)
            {
                Log.Info("[ConsoleInstance] Power toggle: currently powered, toggling off.");
                // If currently running or in standby, toggle off
                PowerOff();
                return;
            }

            PowerOn();

        }
        #endregion
    }
}
