using System;
using System.Runtime.InteropServices;
using UnityEngine;
using RetroLib.Libretro.Utils;

namespace RetroLib.Libretro
{
    public static class LibretroEnvironment
    {
        // Constantes básicas do Libretro
        private const uint RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY = 9;
        private const uint RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY   = 31;

        public static bool OnEnvironment(uint cmd, IntPtr data)
        {
            switch (cmd)
            {
                case RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
                {
                    string path = LibretroPaths.System;
                    IntPtr ptr = Marshal.StringToHGlobalAnsi(path);
                    Marshal.WriteIntPtr(data, ptr);
                    Debug.Log($"[LibretroEnv] System dir: {path}");
                    return true;
                }

                case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
                {
                    string path = LibretroPaths.Saves;
                    IntPtr ptr = Marshal.StringToHGlobalAnsi(path);
                    Marshal.WriteIntPtr(data, ptr);
                    Debug.Log($"[LibretroEnv] Save dir: {path}");
                    return true;
                }

                default:
                    // Importante: retornar false para comandos não suportados
                    return false;
            }
        }
    }
}
