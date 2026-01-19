using System;
using System.Collections;
using System.IO;
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
        [SerializeField] private ConsoleOperationalState operationalState = ConsoleOperationalState.Off;
        [SerializeField] private ConsolePhysicalState physicalState = ConsolePhysicalState.Loose;

        public event Action<ConsoleOperationalState> OnOperationalStateChanged;
        public event Action<ConsolePhysicalState> OnPhysicalStateChanged;

        private bool emulatorDeinitialized = false;

        #endregion

        #region Component References

        private LibretroInstanceVariable emulatorInstance;

        [Header("Screen Reference")]
        [SerializeField] private ScreenInstance screenInstance;

        [Header("Cartridge")]
        private CartridgeInstance insertedCartridge;

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource feedbackAudioSource;
        [SerializeField] private AudioClip clipButton;
        [SerializeField] private AudioClip clipInsertCartridge;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (consoleDefinition != null && displayConsoleName != null)
                displayConsoleName.text = consoleDefinition.consoleName;

            emulatorInstance = ScriptableObject.CreateInstance<LibretroInstanceVariable>();
            emulatorInstance.Current = GetComponent<LibretroInstance>();

            insertedCartridge = null;

            SetOperationalState(ConsoleOperationalState.Off);
            SetPhysicalState(ConsolePhysicalState.Loose);
        }

        #endregion

        #region Screen / Cable Handling

        public void AttachScreen(ScreenInstance screen)
        {
            screenInstance = screen;
            SetPhysicalState(ConsolePhysicalState.ConnectedToScreen);

            // Inform TV: signal is present
            screenInstance.SetSignalPresent(true);
        }

        public void DetachScreen()
        {
            if (screenInstance != null)
            {
                screenInstance.SetSignalPresent(false);
                screenInstance = null;
            }

            SetPhysicalState(ConsolePhysicalState.Loose);
        }

        #endregion

        #region Power Handling

        public void Power()
        {
            if (operationalState == ConsoleOperationalState.Initializing ||
                operationalState == ConsoleOperationalState.ShuttingDown)
                return;

            if (IsPowered())
                PowerOff();
            else
                PowerOn();
        }

        public void PowerOn()
        {
            TryPlayOneShot(clipButton);

            if (consoleDefinition == null)
            {
                Log.Error("[ConsoleInstance] No console definition.");
                return;
            }

            if (screenInstance == null)
            {
                Log.Warn("[ConsoleInstance] PowerOn with no screen attached.");
            }

            if (insertedCartridge == null)
            {
                // Powered but idle
                SetOperationalState(ConsoleOperationalState.Standby);
                NotifyScreenRunning(false);
                return;
            }

            if (operationalState == ConsoleOperationalState.Running)
                return;

            SetOperationalState(ConsoleOperationalState.Initializing);

            if (!InitializeEmulator())
            {
                SetOperationalState(ConsoleOperationalState.Error);
                return;
            }

            emulatorInstance.Current.StartContent();
            StartCoroutine(WaitForEmulatorRunningCoroutine());
        }

        public void PowerOff()
        {
            TryPlayOneShot(clipButton);

            if (!IsPowered())
            {
                SetOperationalState(ConsoleOperationalState.Off);
                NotifyScreenRunning(false);
                return;
            }

            SetOperationalState(ConsoleOperationalState.ShuttingDown);

            if (emulatorInstance?.Current != null && emulatorInstance.Current.Running)
            {
                emulatorInstance.StopContent();
            }

            StartCoroutine(WaitForEmulatorOffCoroutine());
        }

        #endregion

        #region Cartridge Handling

        public void InsertCartridge(SelectEnterEventArgs args)
        {
            if (insertedCartridge != null) return;

            var cartridge = args.interactableObject.transform.GetComponent<CartridgeInstance>();
            if (cartridge == null) return;

            if (!consoleDefinition.supportedExtensions.Contains(cartridge.cartridgeDefinition.extension))
            {
                return;
            }

            insertedCartridge = cartridge;
            SetOperationalState(ConsoleOperationalState.CartridgeInserted);
            TryPlayOneShot(clipInsertCartridge);
        }

        public void EjectCartridge()
        {
            if (insertedCartridge == null) return;

            if (operationalState == ConsoleOperationalState.Running)
                return;

            insertedCartridge = null;
            DeInitializeEmulator();

            SetOperationalState(IsPowered()
                ? ConsoleOperationalState.Standby
                : ConsoleOperationalState.Off);

            NotifyScreenRunning(false);
        }

        #endregion

        #region Emulator Handling

        private bool InitializeEmulator()
        {
            if (insertedCartridge == null || emulatorInstance?.Current == null)
                return false;

            var cartDef = insertedCartridge.cartridgeDefinition;

            string romPath = Path.Combine(
                Application.persistentDataPath,
                "roms",
                cartDef.romSubfolder
            );

            emulatorInstance.Current.Initialize(
                GetCoreName(),
                romPath,
                cartDef.romName
            );

            emulatorInstance.Current.Renderer = screenInstance?.screenRenderer;
            emulatorInstance.Current.Collider = screenInstance?.screenCollider;

            emulatorDeinitialized = false;
            return true;
        }

        private void DeInitializeEmulator()
        {
            if (emulatorDeinitialized) return;
            emulatorDeinitialized = true;

            if (emulatorInstance?.Current != null)
            {
                emulatorInstance.Current.DeInitialize();
                emulatorInstance.Current.Renderer = null;
            }
        }

        private string GetCoreName()
        {
            return string.IsNullOrEmpty(insertedCartridge.cartridgeDefinition.overrideCoreName)
                ? consoleDefinition.coreName
                : insertedCartridge.cartridgeDefinition.overrideCoreName;
        }

        #endregion

        #region State Helpers

        private void NotifyScreenRunning(bool running)
        {
            if (screenInstance != null)
                screenInstance.SetConsoleRunning(running);
        }

        public void SetOperationalState(ConsoleOperationalState newState)
        {
            if (operationalState == newState) return;

            operationalState = newState;
            OnOperationalStateChanged?.Invoke(operationalState);

            switch (operationalState)
            {
                case ConsoleOperationalState.Initializing:
                    NotifyScreenRunning(false);
                    break;

                case ConsoleOperationalState.Running:
                    NotifyScreenRunning(true);
                    break;

                case ConsoleOperationalState.Off:
                case ConsoleOperationalState.Standby:
                case ConsoleOperationalState.Error:
                    NotifyScreenRunning(false);
                    break;
            }
        }

        public void SetPhysicalState(ConsolePhysicalState newState)
        {
            if (physicalState == newState) return;

            physicalState = newState;
            OnPhysicalStateChanged?.Invoke(physicalState);
        }

        private bool IsPowered()
        {
            return operationalState != ConsoleOperationalState.Off &&
                   operationalState != ConsoleOperationalState.Error;
        }

        private void TryPlayOneShot(AudioClip clip)
        {
            if (!playAudio || feedbackAudioSource == null || clip == null) return;
            feedbackAudioSource.PlayOneShot(clip);
        }

        #endregion

        #region Coroutines

        private IEnumerator WaitForEmulatorRunningCoroutine(float timeout = 5f)
        {
            float t = 0f;
            while (t < timeout)
            {
                if (emulatorInstance.Current != null &&
                    emulatorInstance.Current.Running)
                {
                    SetOperationalState(ConsoleOperationalState.Running);
                    yield break;
                }

                t += Time.deltaTime;
                yield return null;
            }

            SetOperationalState(ConsoleOperationalState.Error);
        }

        private IEnumerator WaitForEmulatorOffCoroutine(float timeout = 5f)
        {
            float t = 0f;
            while (t < timeout)
            {
                if (emulatorInstance.Current != null &&
                    !emulatorInstance.Current.Running)
                {
                    DeInitializeEmulator();
                    SetOperationalState(ConsoleOperationalState.Off);
                    yield break;
                }

                t += Time.deltaTime;
                yield return null;
            }

            SetOperationalState(ConsoleOperationalState.Error);
        }

        #endregion
    }
}
