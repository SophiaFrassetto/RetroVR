using System;
using System.Runtime.InteropServices;

namespace RetroLib.Libretro.Native
{
    public static class LibretroNative
    {

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
