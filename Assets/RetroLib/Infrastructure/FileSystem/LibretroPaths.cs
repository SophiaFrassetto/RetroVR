using RetroLib.Infrastructure.FileSystem;

namespace RetroLib.Infrastructure.FileSystem
{
    public static class LibretroPaths
    {
        public static string System => RetroFileSystem.System;
        public static string Saves => RetroFileSystem.Saves;

        public static string GetCorePath(string core)
            => RetroFileSystem.GetCorePath(core);

        public static string GetRomPath(string system, string rom)
            => RetroFileSystem.GetRomPath(system, rom);
    }
}
