using SK.Libretro.Unity;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    [RequireComponent(typeof(LibretroInstance)), RequireComponent(typeof(AudioSource))]
    public class console : MonoBehaviour
    {

        private LibretroInstanceVariable emulatorInstance;

        [SerializeField]
        private string coreName;

        private cartridge attachCartridge;

        void Awake()
        {
            emulatorInstance = ScriptableObject.CreateInstance<LibretroInstanceVariable>();
            emulatorInstance.Current = GetComponent<LibretroInstance>();
        }

        public void AttachCartridge(SelectEnterEventArgs args)
        {
            if (attachCartridge != null) return;
            var cartridge = args.interactableObject.transform.GetComponent<cartridge>();
            if (cartridge == null) return;
            attachCartridge = cartridge;
            Debug.Log($"Cartridge {cartridge.romName} attached to console");
        }

        public void PowerOn()
        {
            Debug.Log("Powering on console");
            emulatorInstance.Current.Initialize(
                coreName,
                attachCartridge.romPath,
                attachCartridge.romName
            );
            emulatorInstance.Current.StartContent();
        }
    }
}
