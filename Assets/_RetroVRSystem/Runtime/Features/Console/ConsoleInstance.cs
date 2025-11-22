using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using SK.Libretro.Unity;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    [RequireComponent(typeof(LibretroInstance))]
    public class ConsoleInstance : MonoBehaviour
    {
        #region Fields
        [Header("Libretro Instance")]
        private LibretroInstanceVariable emulatorInstance;

        [Header("Console Configuration")]
        [SerializeField] private ConsoleDefinition consoleDefinition;
        [SerializeField] private ScreenInstance screenInstance;
        [SerializeField] private TMP_Text displayConsoleName;

        [Header("Cartridge Configuration")]
        private CartridgeInstance insertedCartridge;

        [Header("Console State")]
        [SerializeField] private bool running => GetRunningState();

        [SerializeField]
        private ConsoleOperationalState operationalState = ConsoleOperationalState.Off;

        [SerializeField]
        private ConsolePhysicalState physicalState = ConsolePhysicalState.Loose;

        /// <summary>
        /// Notifies listeners when operational state changes (Initializing, Running, Error, etc.)
        /// </summary>
        public event Action<ConsoleOperationalState> OnOperationalStateChanged;

        /// <summary>
        /// Notifies listeners when physical state changes (Held, Placed, Connected, etc.)
        /// </summary>
        public event Action<ConsolePhysicalState> OnPhysicalStateChanged;

        // Optional audio feedback (serialize if you want sounds; keep null-safe)
        [Header("Optional Feedback (audio)")]
        [SerializeField] private AudioSource feedbackAudioSource;
        [SerializeField] private AudioClip clipPowerOn;
        [SerializeField] private AudioClip clipInsertCartridge;
        [SerializeField] private AudioClip clipPowerOff;
        #endregion

        #region Execution
        private void Awake()
        {
            if (consoleDefinition != null)
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

        #region Console Management
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

        #region Power Management
        public void PowerOn()
        {
            if (consoleDefinition == null)
            {
                Log.Error("[ConsoleInstance] Console definition is not set.");
                return;
            }

            if (insertedCartridge == null)
            {
                Log.Error("[ConsoleInstance] No cartridge inserted.");
                return;
            }

            SetOperationalState(ConsoleOperationalState.Initializing);
            emulatorInstance.StartContent();

            // wait for libretro to report running
            StartCoroutine(WaitForEmulatorRunningCoroutine(5f));
        }

        public void PowerOff()
        {
            if (!running || !emulatorInstance.Current.Running)
            {
                Log.Error("[ConsoleInstance] Console is not powered on or emulator is not running.");
                return;
            }

            SetOperationalState(ConsoleOperationalState.ShuttingDown);
            emulatorInstance.StopContent();
            SetOperationalState(ConsoleOperationalState.Off);
        }

        public void ResetConsole()
        {
            if (!running || !emulatorInstance.Current.Running)
            {
                Log.Error("[ConsoleInstance] Console is not powered on or emulator is not running.");
                return;
            }
            PowerOff();
            PowerOn();
        }
        #endregion

        #region Cartridge Management
        public void InsertCartridge(SelectEnterEventArgs args)
        {
            if (running)
            {
                Log.Error("[ConsoleInstance] Cannot insert cartridge while console is powered on.");
                return;
            }

            if (insertedCartridge != null)
            {
                Log.Warn("[ConsoleInstance] A cartridge is already inserted. Ejecting the current cartridge.");
                return;
            }

            var interactorable = args.interactableObject;
            var cartridgeObject = interactorable.transform.gameObject;
            CartridgeInstance cartridgeInstance = cartridgeObject.GetComponent<CartridgeInstance>();

            if (cartridgeInstance.cartridgeDefinition == null)
            {
                Log.Error("[ConsoleInstance] Cannot insert a null cartridge.");
                return;
            }
            else
            {
                if (CanAcceptCartridge(cartridgeInstance.cartridgeDefinition))
                {
                    insertedCartridge = cartridgeInstance;
                    SetPhysicalState(ConsolePhysicalState.Placed);
                    SetOperationalState(ConsoleOperationalState.CartridgeInserted);
                    InitializeEmulator();
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

            if (running)
            {
                PowerOff();
                Log.Warn("[ConsoleInstance] Console is powered off before ejecting cartridge.");
            }

            insertedCartridge = null;
            emulatorInstance.Current.DeInitialize();
            SetOperationalState(ConsoleOperationalState.Off);
            SetPhysicalState(ConsolePhysicalState.Loose);
        }
        #endregion

        #region Emulator Management\
        private void InitializeEmulator()
        {
            if (emulatorInstance == null || consoleDefinition == null)
            {
                Log.Error("[ConsoleInstance] Emulator instance or console definition is not set.");
                return;
            }

            emulatorInstance.Current.Initialize(CoreToUse(), insertedCartridge.cartridgeDefinition.romsDirectory, insertedCartridge.cartridgeDefinition.romName);
            emulatorInstance.Current.Renderer = screenInstance.screenRenderer;
            emulatorInstance.Current.Collider = screenInstance.screenCollider;
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

        #region Input Management
        #endregion

        #region State Management
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
                    TryPlayOneShot(clipPowerOn);
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
        #endregion
    }
}
