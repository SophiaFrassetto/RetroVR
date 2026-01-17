using System;
using System.Runtime.InteropServices;
using UnityEngine;
using RetroLib.Core;
using RetroLib.Libretro.Native;
using RetroLib.Debugging;

namespace RetroLib.Libretro
{
    public class RetroCoreLibretro : IRetroCore
    {
        private Texture2D videoTexture;
        private LibretroCoreState state = LibretroCoreState.Created;

        private static byte[] frameBuffer;
        private static int frameWidth;
        private static int frameHeight;

        private LibretroNative.VideoRefreshCallback videoCallback;

        public bool LoadCore(string corePath)
        {
            Debug.Log("[Libretro] Setting environment");
            LibretroNative.retro_set_environment(LibretroEnvironment.OnEnvironment);
            state = LibretroCoreState.EnvironmentReady;

            Debug.Log("[Libretro] Init core");
            LibretroNative.retro_init();
            state = LibretroCoreState.CoreInitialized;

            videoCallback = OnVideoRefresh;
            LibretroNative.retro_set_video_refresh(videoCallback);

            uint api = LibretroNative.retro_api_version();
            Debug.Log($"[Libretro] API version: {api}");

            LibretroNative.retro_system_info sysInfo;
            LibretroNative.retro_get_system_info(out sysInfo);

            string libName = Marshal.PtrToStringAnsi(sysInfo.library_name);
            Debug.Log($"[Libretro] System: {libName}");
            string libVersion = Marshal.PtrToStringAnsi(sysInfo.library_version);
            Debug.Log($"[Libretro] Version: {libVersion}");
            string extensions = Marshal.PtrToStringAnsi(sysInfo.valid_extensions);
            Debug.Log($"[Libretro] Extensions: {extensions}");

            Debug.Log($"[Libretro] Needs Full Path: {sysInfo.need_fullpath}");

            return true;
        }

        public bool LoadRom(string romPath)
        {
            Debug.Log($"[Libretro] Loading rom: {romPath}");

            var game = new LibretroNative.retro_game_info
            {
                path = Marshal.StringToHGlobalAnsi(romPath),
                data = IntPtr.Zero,
                size = UIntPtr.Zero,
                meta = IntPtr.Zero
            };

            bool result = LibretroNative.retro_load_game(ref game);

            Marshal.FreeHGlobal(game.path);

            Debug.Log($"[Libretro] Rom loaded: {result}");
            return result;
        }

        public void StartEmulation()
        {
            if (state < LibretroCoreState.CoreInitialized)
            {
                Debug.LogWarning("[Libretro] Cannot start emulation yet");
                return;
            }

            state = LibretroCoreState.Running;
            DebugStats.CoreRunning = true;
        }

        public void StopEmulation()
        {
            Debug.Log("[Libretro] Stop");
            state = LibretroCoreState.Created;
            DebugStats.CoreRunning = false;
            LibretroNative.retro_deinit();
        }

        public bool IsRunning => state == LibretroCoreState.Running;

        // 🔑 EXECUTION SEPARADO
        public bool RunFrame()
        {
            if (state != LibretroCoreState.Running)
                return false;

            LibretroNative.retro_run();
            return true;
        }

        // 🎥 APENAS RETORNA O FRAME
        public Texture GetVideoTexture()
        {
            if (frameWidth <= 0 || frameHeight <= 0)
                return null;

            if (videoTexture == null)
            {
                videoTexture = new Texture2D(
                    frameWidth,
                    frameHeight,
                    TextureFormat.RGBA32,
                    false
                );
                videoTexture.filterMode = FilterMode.Point;
            }

            if (frameBuffer != null)
            {
                videoTexture.LoadRawTextureData(frameBuffer);
                videoTexture.Apply();
            }

            return videoTexture;
        }

        // ===== Video Callback =====
        private static void OnVideoRefresh(
            IntPtr data,
            uint width,
            uint height,
            ulong pitch
        )
        {
            int size = (int)(height * pitch);

            if (frameBuffer == null || frameBuffer.Length != size)
                frameBuffer = new byte[size];

            Marshal.Copy(data, frameBuffer, 0, size);

            frameWidth = (int)width;
            frameHeight = (int)height;

            DebugStats.VideoWidth = frameWidth;
            DebugStats.VideoHeight = frameHeight;
        }

        // ===== Audio placeholders =====
        public int GetSampleRate() => 32040;
        public int GetAudioChannels() => 2;
        public float[] GetAudioBuffer() => null;
    }
}
