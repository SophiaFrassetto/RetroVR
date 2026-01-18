using System.IO;
using UnityEngine;

namespace RetroLib.Infrastructure.FileSystem
{
    public static class RetroFileSystem
    {
        public static string Root { get; private set; }

        public static string Cores => Path.Combine(Root, "cores");
        public static string Roms => Path.Combine(Root, "roms");
        public static string System => Path.Combine(Root, "system");
        public static string Saves => Path.Combine(Root, "saves");
        public static string States => Path.Combine(Root, "states");
        public static string Logs => Path.Combine(Root, "logs");

        public static void Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Root = Path.Combine(Application.persistentDataPath, "retro");
#else
            Root = Path.Combine(Application.dataPath, "../libretro");
#endif
            CreateIfMissing(Root);
            CreateIfMissing(Cores);
            CreateIfMissing(Roms);
            CreateIfMissing(System);
            CreateIfMissing(Saves);
            CreateIfMissing(States);
            CreateIfMissing(Logs);
        }

        private static void CreateIfMissing(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        // ================= HELPERS =================

        public static string GetCorePath(string coreFile)
        {
            return Path.Combine(Cores, coreFile);
        }

        public static string GetRomPath(string system, string romFile)
        {
            string dir = Path.Combine(Roms, system);
            CreateIfMissing(dir);
            return Path.Combine(dir, romFile);
        }
    }
}
