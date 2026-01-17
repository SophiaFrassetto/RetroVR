using System.IO;
using UnityEngine;

namespace RetroLib.Libretro.Utils
{
    public static class LibretroPaths
    {
        /// <summary>
        /// Pasta raiz do libretro fora do Assets
        /// </summary>
        public static string Root
        {
            get
            {
                // ProjectRoot/libretro
                return Path.Combine(
                    Application.dataPath,
                    "..",
                    "libretro"
                );
            }
        }

        public static string Cores =>
            Path.Combine(Root, "cores");

        public static string Roms =>
            Path.Combine(Root, "roms");

        public static string Saves =>
            Path.Combine(Root, "saves");

        public static string System =>
            Path.Combine(Root, "system");

        /// <summary>
        /// Retorna o caminho completo da ROM
        /// Ex: GetRomPath("snes", "Super Mario World.smc")
        /// </summary>
        public static string GetRomPath(string system, string romFile)
        {
            string path = Path.Combine(Roms, system, romFile);

            if (!File.Exists(path))
            {
                Debug.LogError($"[LibretroPaths] ROM not found: {path}");
            }

            return path;
        }

        /// <summary>
        /// Retorna o caminho do core
        /// </summary>
        public static string GetCorePath(string coreFile)
        {
            string path = Path.Combine(Cores, coreFile);

            if (!File.Exists(path))
            {
                Debug.LogError($"[LibretroPaths] Core not found: {path}");
            }

            return path;
        }
    }
}
