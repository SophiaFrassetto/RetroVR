using System;
using System.Runtime.InteropServices;
using RetroLib.Infrastructure.FileSystem;
using RetroLib.LibretroHost.Native;
using RetroLib.LibretroHost.State;


namespace RetroLib.LibretroHost.Environment
{
    public static class LibretroEnvironment
    {
        public static LibretroCoreState State { get; private set; }

        private static IntPtr systemDirPtr = IntPtr.Zero;
        private static IntPtr saveDirPtr = IntPtr.Zero;

        public static void BindState(LibretroCoreState coreState)
        {
            State = coreState;
        }

        // libretro.h - retro_environment_cmd
        private const uint RETRO_ENVIRONMENT_SET_ROTATION = 1;
        private const uint RETRO_ENVIRONMENT_GET_OVERSCAN = 2;
        private const uint RETRO_ENVIRONMENT_GET_CAN_DUPE = 3;
        private const uint RETRO_ENVIRONMENT_SET_MESSAGE = 6;
        private const uint RETRO_ENVIRONMENT_SHUTDOWN = 7;
        private const uint RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL = 8;
        private const uint RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY = 9;
        private const uint RETRO_ENVIRONMENT_SET_PIXEL_FORMAT = 10;
        private const uint RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS = 11;
        private const uint RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK = 12;
        private const uint RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE = 13;
        private const uint RETRO_ENVIRONMENT_SET_HW_RENDER = 14;
        private const uint RETRO_ENVIRONMENT_GET_VARIABLE = 15;
        private const uint RETRO_ENVIRONMENT_SET_VARIABLES = 16;
        private const uint RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE = 17;
        private const uint RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME = 18;
        private const uint RETRO_ENVIRONMENT_GET_LIBRETRO_PATH = 19;
        private const uint RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK = 21;
        private const uint RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK = 22;
        private const uint RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE = 23;
        private const uint RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES = 24;
        private const uint RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE = 25;
        private const uint RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE = 26;
        private const uint RETRO_ENVIRONMENT_GET_LOG_INTERFACE = 27;
        private const uint RETRO_ENVIRONMENT_GET_PERF_INTERFACE = 28;
        private const uint RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE = 29;
        private const uint RETRO_ENVIRONMENT_GET_CONTENT_DIRECTORY = 30;
        private const uint RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY = 31;
        private const uint RETRO_ENVIRONMENT_GET_SYSTEM_AV_INFO = 32;
        private const uint RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK = 33;
        private const uint RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO = 34;
        private const uint RETRO_ENVIRONMENT_SET_CONTROLLER_INFO = 35;
        private const uint RETRO_ENVIRONMENT_SET_MEMORY_MAPS = 36;
        private const uint RETRO_ENVIRONMENT_SET_GEOMETRY = 37;
        private const uint RETRO_ENVIRONMENT_GET_USERNAME = 38;
        private const uint RETRO_ENVIRONMENT_GET_LANGUAGE = 39;
        private const uint RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER = 40;
        private const uint RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE = 41;
        private const uint RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS = 42;

        private const int RETRO_PIXEL_FORMAT_XRGB8888 = 1;
        private const int RETRO_PIXEL_FORMAT_RGB565 = 2;

        public static bool OnEnvironment(uint cmd, IntPtr data)
        {
            if (State == null)
                return false;

            switch (cmd)
            {
                case RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
                {
                    int fmt = Marshal.ReadInt32(data);
                    lock (State.Sync)
                    {
                        State.PixelFormat =
                            fmt == RETRO_PIXEL_FORMAT_RGB565 ? "RGB565" : "XRGB8888";
                    }
                    return true;
                }

                case RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
                {
                    if (systemDirPtr == IntPtr.Zero)
                        systemDirPtr = Marshal.StringToHGlobalAnsi(LibretroPaths.System);

                    Marshal.WriteIntPtr(data, systemDirPtr);
                    return true;
                }

                case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
                {
                    if (saveDirPtr == IntPtr.Zero)
                        saveDirPtr = Marshal.StringToHGlobalAnsi(LibretroPaths.Saves);

                    Marshal.WriteIntPtr(data, saveDirPtr);
                    return true;
                }

                case RETRO_ENVIRONMENT_GET_SYSTEM_AV_INFO:
                {
                    var avInfo = Marshal.PtrToStructure<LibretroNative.retro_system_av_info>(data);

                    State.VideoWidth = (int)avInfo.geometry.base_width;
                    State.VideoHeight = (int)avInfo.geometry.base_height;
                    State.AudioSampleRate = (int)avInfo.timing.sample_rate;
                    State.TargetFps = (float)avInfo.timing.fps;

                    return true;
                }
            }

            return false;
        }

        public static void Cleanup()
        {
            if (systemDirPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(systemDirPtr);
                systemDirPtr = IntPtr.Zero;
            }

            if (saveDirPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(saveDirPtr);
                saveDirPtr = IntPtr.Zero;
            }
        }
    }
}
