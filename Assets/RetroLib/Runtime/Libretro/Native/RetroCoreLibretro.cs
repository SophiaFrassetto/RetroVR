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
        private bool running;

        private static byte[] frameBuffer;
        private static int frameWidth;
        private static int frameHeight;

        private LibretroNative.VideoRefreshCallback videoCallback;

        public bool LoadCore(string corePath)
        {
            // 1. Registrar environment PRIMEIRO
            LibretroNative.retro_set_environment(LibretroEnvironment.OnEnvironment);

            // 2. Agora sim inicializar o core
            LibretroNative.retro_init();

            // 3. Registrar callbacks de vídeo
            videoCallback = OnVideoRefresh;
            LibretroNative.retro_set_video_refresh(videoCallback);

            // 4. Debug
            uint api = LibretroNative.retro_api_version();
            Debug.Log($"[Libretro] API version: {api}");

            videoCallback = OnVideoRefresh;
            LibretroNative.retro_set_video_refresh(videoCallback);

            return true;
        }

        public bool LoadRom(string romPath)
        {
            // Ainda não implementado (retro_load_game)
            Debug.Log("[Libretro] ROM loading skipped for now");
            return true;
        }

        public void StartEmulation()
        {
            running = true;
        }

        public void StopEmulation()
        {
            running = false;
            LibretroNative.retro_deinit();
        }

        public bool IsRunning => running;

        public Texture GetVideoTexture()
        {
            if (!running)
                return null;

            LibretroNative.retro_run();

            if (videoTexture == null && frameWidth > 0)
            {
                videoTexture = new Texture2D(
                    frameWidth,
                    frameHeight,
                    TextureFormat.RGBA32,
                    false
                );
                videoTexture.filterMode = FilterMode.Point;
            }

            if (videoTexture != null && frameBuffer != null)
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
