using SK.Libretro.Unity;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    [RequireComponent(typeof(LibretroInstance)), RequireComponent(typeof(AudioSource))]
    public class Core : MonoBehaviour
    {

        private LibretroInstanceVariable emulatorInstance;
        private AudioSource audioSource;

        public string coreName;
        public GameObject consolePrefab;


        void Awake()
        {
            emulatorInstance = ScriptableObject.CreateInstance<LibretroInstanceVariable>();
            emulatorInstance.Current = GetComponent<LibretroInstance>();
        }
    }
}
