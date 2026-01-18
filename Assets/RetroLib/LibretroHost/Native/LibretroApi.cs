using System;
using System.Runtime.InteropServices;

namespace RetroLib.LibretroHost.Native
{
    public sealed class LibretroApi
    {
        // =========================
        // DELEGATES
        // =========================

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_init_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_deinit_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool retro_load_game_t(ref retro_game_info game);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_run_t();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_get_system_info_t(out retro_system_info info);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_environment_t(EnvironmentCallback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_video_refresh_t(VideoRefreshCallback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_audio_sample_batch_t(AudioSampleBatchCallback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_input_poll_t(InputPollCallback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void retro_set_input_state_t(InputStateCallback cb);

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
        public struct retro_game_geometry
        {
            public uint base_width;
            public uint base_height;
            public uint max_width;
            public uint max_height;
            public float aspect_ratio;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct retro_system_av_info
        {
            public retro_game_geometry geometry;
            public retro_system_timing timing;
        }

        // =========================
        // CALLBACKS
        // =========================

        public delegate bool EnvironmentCallback(uint cmd, IntPtr data);
        public delegate void VideoRefreshCallback(IntPtr data, uint w, uint h, ulong pitch);
        public delegate UIntPtr AudioSampleBatchCallback(IntPtr data, UIntPtr frames);
        public delegate void InputPollCallback();
        public delegate short InputStateCallback(uint port, uint device, uint index, uint id);

        // =========================
        // RESOLVED SYMBOLS
        // =========================

        public retro_init_t retro_init;
        public retro_deinit_t retro_deinit;
        public retro_load_game_t retro_load_game;
        public retro_run_t retro_run;
        public retro_get_system_info_t retro_get_system_info;

        public retro_set_environment_t retro_set_environment;
        public retro_set_video_refresh_t retro_set_video_refresh;
        public retro_set_audio_sample_batch_t retro_set_audio_sample_batch;
        public retro_set_input_poll_t retro_set_input_poll;
        public retro_set_input_state_t retro_set_input_state;
    }
}
