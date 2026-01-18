using UnityEngine;
using RetroLib.Engine.Manager;
using RetroLib.Engine.Core;
using RetroLib.Infrastructure.FileSystem;

namespace RetroLib.Sandbox
{
    public class SandboxBootstrap : MonoBehaviour
    {
        [SerializeField] private RetroLibManager runtime;

        private RetroSession session;
        private bool started;

        private void Start()
        {
            Debug.Log("[Sandbox] Ready. Press F5 to start Libretro.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartEmulation();
            }
        }

        private void StartEmulation()
        {
            if (started)
                return;

            started = true;

            session = runtime.CreateSession();

            session.LoadCore(
                LibretroPaths.GetCorePath("snes9x_libretro.dll")
            );

            session.LoadRom(
                LibretroPaths.GetRomPath(
                    "snes",
                    "Super Mario World (U) [!].smc"
                )
            );

            session.Start();

            Debug.Log("[Sandbox] Emulation started");
        }
    }
}
