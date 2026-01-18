using System;
using System.Runtime.InteropServices;

namespace RetroLib.LibretroHost.Native
{
    public static class LibretroNative
    {
        // =========================
        // STRUCTS
        // =========================
        [StructLayout(LayoutKind.Sequential)]
        public struct retro_game_info
        {
            public IntPtr path;
            public IntPtr data;
            public UIntPtr size;
            public IntPtr meta;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_timing
        {
            public double fps;
            public double sample_rate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_av_info
        {
            public retro_game_geometry geometry;
            public retro_system_timing timing;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_game_geometry
        {
            public uint base_width;
            public uint base_height;
            public uint max_width;
            public uint max_height;
            public float aspect_ratio;
        }

        // =========================
        // CORE INFO
        // =========================
        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_get_system_info(out retro_system_info info);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint retro_api_version();

        // =========================
        // GAME
        // =========================
        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool retro_load_game(ref retro_game_info game);

        // =========================
        // LIFECYCLE
        // =========================
        public delegate bool EnvironmentCallback(uint cmd, IntPtr data);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_environment(EnvironmentCallback cb);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_init();

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_deinit();

        // =========================
        // VIDEO
        // =========================
        public delegate void VideoRefreshCallback(
            IntPtr data,
            uint width,
            uint height,
            ulong pitch
        );

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_video_refresh(VideoRefreshCallback cb);

        // =========================
        // INPUT (OBRIGATÓRIO)
        // =========================
        public delegate void InputPollCallback();

        public delegate short InputStateCallback(
            uint port,
            uint device,
            uint index,
            uint id
        );

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_input_poll(InputPollCallback cb);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_input_state(InputStateCallback cb);

        // =========================
        // AUDIO (OBRIGATÓRIO)
        // =========================
        public delegate void AudioSampleCallback(short left, short right);

        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_audio_sample(AudioSampleCallback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UIntPtr AudioSampleBatchCallback(
            IntPtr data,
            UIntPtr frames
        );
        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_set_audio_sample_batch(
            AudioSampleBatchCallback cb
        );
        // =========================
        // MAIN LOOP
        // =========================
        [DllImport("snes9x_libretro", CallingConvention = CallingConvention.Cdecl)]
        public static extern void retro_run();
    }
}
