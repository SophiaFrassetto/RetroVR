using UnityEngine;
using TMPro;
using System.Linq;
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

            emulatorInstance.StartContent();
        }

        public void PowerOff()
        {
            if (!running || !emulatorInstance.Current.Running)
            {
                Log.Error("[ConsoleInstance] Console is not powered on or emulator is not running.");
                return;
            }

            emulatorInstance.StopContent();
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
        }
        #endregion

        #region Emulator Management
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
    }
}
