using UnityEngine;
using RetroLib.Manager;
using RetroLib.Libretro;
using RetroLib.Libretro.Utils;

namespace RetroLib.Sandbox
{
    public class SandboxBootstrapLibretro : MonoBehaviour
    {
        [SerializeField] private RetroLibManager manager;

        void Start()
        {
            var core = new RetroCoreLibretro();

            core.LoadCore("snes9x_libretro.dll");
            core.LoadRom("dummy");

            core.StartEmulation();

            manager.SetCore(core);
        }
    }
}
