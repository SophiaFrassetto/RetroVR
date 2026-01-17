using UnityEngine;
using RetroLib.Core;
using RetroLib.Manager;

namespace RetroLib.Sandbox
{
    public class SandboxBootstrap : MonoBehaviour
    {
        [SerializeField] private RetroLibManager manager;

        void Start()
        {
            var mock = new MockRetroCore();
            mock.LoadCore("MockCore");
            mock.LoadRom("MockRom");
            mock.StartEmulation();

            manager.SetCore(mock);
        }
    }
}
