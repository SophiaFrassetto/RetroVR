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
        private const uint RETRO_ENVIRONMENT_SET_PIXEL_FORMAT = 10;
        private const int RETRO_PIXEL_FORMAT_XRGB8888 = 1;

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

                case RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
                {
                    int requestedFormat = Marshal.ReadInt32(data);

                    switch (requestedFormat)
                    {
                        case 1: // XRGB8888
                            Debug.Log("[LibretroEnv] Pixel format accepted: XRGB8888");
                            return true;

                        case 2: // RGB565
                            Debug.Log("[LibretroEnv] Pixel format accepted: RGB565");
                            return true;

                        default:
                            Debug.LogWarning($"[LibretroEnv] Unsupported pixel format: {requestedFormat}");
                            return false;
                    }
                }

                default:
                    // Importante: retornar false para comandos não suportados
                    return false;
            }
        }
    }
}
