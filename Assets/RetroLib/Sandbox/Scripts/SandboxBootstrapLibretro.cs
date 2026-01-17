using UnityEngine;
using RetroLib.Manager;
using RetroLib.Libretro;
using RetroLib.Libretro.Utils;

namespace RetroLib.Sandbox
{
    public class SandboxBootstrapLibretro : MonoBehaviour
    {
        [SerializeField] private RetroLibManager manager;

        private RetroCoreLibretro core;

        void Awake()
        {
            if (manager == null)
            {
                Debug.LogError("[SandboxBootstrapLibretro] Manager not assigned");
                enabled = false;
                return;
            }

            core = new RetroCoreLibretro();
            manager.SetCore(core);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartLibretro();
            }
        }

        void Start()
        {
            // ⚠️ NÃO inicializar Libretro pesado aqui automaticamente
            Debug.Log("[SandboxBootstrapLibretro] Ready. Waiting for manual start.");
        }

        // 🔑 Chame isso manualmente depois (botão, tecla, menu)
        public void StartLibretro()
        {
            Debug.Log("[SandboxBootstrapLibretro] Starting Libretro");

            core.LoadCore("snes9x_libretro.dll");

            string romPath = LibretroPaths.GetRomPath("snes", "Super Mario World (U) [!].smc");

            core.LoadRom(romPath);

            Debug.Log("[SandboxBootstrapLibretro] Game loaded, ready to run");
        }
    }
}
