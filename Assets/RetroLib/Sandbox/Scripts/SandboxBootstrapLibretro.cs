using UnityEngine;
using RetroLib.Manager;
using RetroLib.Libretro;
using RetroLib.Libretro.Utils;

namespace RetroLib.Sandbox
{
    public class SandboxBootstrapLibretro : MonoBehaviour
    {
        [SerializeField] private RetroLibManager manager;
        private bool started = false;

        private void Start()
        {
            manager = RetroLibManager.Instance;
            Debug.Log("[SandboxBootstrapLibretro] Ready. Waiting for manual start.");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartLibretro();
            }
        }

        public void StartLibretro()
        {
            if (started)
            {
                Debug.Log("[Sandbox] Libretro already started");
                return;
            }
            started = true;

            if (manager == null)
            {
                Debug.LogWarning("[Sandbox] Manager/Core not ready");
                return;
            }

            Debug.Log("[SandboxBootstrapLibretro] Starting Libretro");
            manager.CreateCore();

            var core = manager.core;
            if (core == null)
            {
                Debug.LogError("[Sandbox] Core not available");
                return;
            }

            Debug.Log("[SandboxBootstrapLibretro] Starting Libretro");

            if (!core.LoadCore("snes9x_libretro.dll"))
                return;

            if (!core.LoadRom(
                LibretroPaths.GetRomPath(
                    "snes",
                    "Super Mario World (U) [!].smc")))
                return;

            core.StartEmulation();
        }
    }
}
