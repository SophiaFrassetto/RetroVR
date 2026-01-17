using System.IO;
using UnityEngine;

namespace RetroLib.Libretro.Utils
{
    public static class LibretroPaths
    {
        public static string Root
        {
            get
            {
                // ProjectRoot/Libretro
                return Path.GetFullPath(
                    Path.Combine(Application.dataPath, "..", "libretro")
                );
            }
        }

        public static string Cores => Path.Combine(Root, "cores");
        public static string Roms => Path.Combine(Root, "roms");
        public static string Saves => Path.Combine(Root, "saves");
        public static string System => Path.Combine(Root, "system");

        public static string GetCore(string name)
            => Path.Combine(Cores, name);

        public static string GetRom(string subfolder, string rom)
            => Path.Combine(Roms, subfolder, rom);
    }
}
