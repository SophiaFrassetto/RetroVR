using System;
using System.Runtime.InteropServices;

namespace RetroLib.Libretro.Native
{
    public static class LibretroNative
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct retro_game_info
        {
            public IntPtr path;
            public IntPtr data;
            public UIntPtr size;
            public IntPtr meta;
        }

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool retro_load_game(ref retro_game_info game);

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_info
        {
            public IntPtr library_name;
            public IntPtr library_version;
            public IntPtr valid_extensions;

            [MarshalAs(UnmanagedType.I1)]
            public bool need_fullpath;

            [MarshalAs(UnmanagedType.I1)]
            public bool block_extract;
        }

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_get_system_info(out retro_system_info info);

        // ===== Lifecycle =====
        public delegate bool EnvironmentCallback(
            uint cmd,
            IntPtr data
        );

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_environment(EnvironmentCallback cb);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_init();

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_deinit();

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint retro_api_version();

        // ===== Callbacks =====

        public delegate void VideoRefreshCallback(
            IntPtr data,
            uint width,
            uint height,
            ulong pitch
        );

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_video_refresh(VideoRefreshCallback cb);

        // ===== Main Loop =====

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_run();
    }
}
