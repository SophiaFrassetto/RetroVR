using System;
using System.Runtime.InteropServices;
using UnityEngine;
using RetroLib.Core;
using RetroLib.Libretro.Native;

namespace RetroLib.Libretro
{
    public class RetroCoreLibretro : IRetroCore
    {
        private LibretroCoreState state;

        // vídeo
        private byte[] videoBuffer;
        private int videoWidth;
        private int videoHeight;
        private bool frameReady;

        private Texture2D videoTexture;

        // callbacks mantidos vivos
        private LibretroNative.VideoRefreshCallback videoCb;
        private LibretroNative.InputPollCallback inputPollCb;
        private LibretroNative.InputStateCallback inputStateCb;
        private LibretroNative.AudioSampleCallback audioCb;
        // áudio
        private float[] audioBuffer;
        private int audioFrames;
        private bool audioReady;
        private int sampleRate;

        private const int AUDIO_RING_SIZE = 44100 * 2; // 1 segundo stereo
        private float[] audioRing = new float[AUDIO_RING_SIZE];
        private int audioWritePos;
        private int audioReadPos;


        // manter delegate vivo (CRÍTICO)
        private LibretroNative.AudioSampleBatchCallback audioBatchCb;
        public void SetState(LibretroCoreState coreState)
        {
            state = coreState;
        }

        public bool LoadCore(string corePath)
        {
            if (state == null) return false;

            LibretroEnvironment.BindState(state);
            LibretroNative.retro_set_environment(LibretroEnvironment.OnEnvironment);

            // callbacks mínimos obrigatórios
            videoCb = OnVideoRefresh;
            inputPollCb = () => { };
            inputStateCb = (a, b, c, d) => 0;
            audioCb = (l, r) => { };

            LibretroNative.retro_set_video_refresh(videoCb);
            LibretroNative.retro_set_input_poll(inputPollCb);
            LibretroNative.retro_set_input_state(inputStateCb);
            LibretroNative.retro_set_audio_sample(audioCb);

            audioBatchCb = OnAudioSampleBatch;
            LibretroNative.retro_set_audio_sample_batch(audioBatchCb);

            LibretroNative.retro_init();

            sampleRate = state.AudioSampleRate;
            state.Lifecycle = LibretroLifecycleState.CoreInitialized;
            return true;
        }

        public bool LoadRom(string romPath)
        {
            if (state.Lifecycle != LibretroLifecycleState.CoreInitialized)
                return false;

            var game = new LibretroNative.retro_game_info
            {
                path = Marshal.StringToHGlobalAnsi(romPath),
                data = IntPtr.Zero,
                size = UIntPtr.Zero,
                meta = IntPtr.Zero
            };

            bool ok = LibretroNative.retro_load_game(ref game);
            Marshal.FreeHGlobal(game.path);

            state.GameLoaded = ok;
            state.Lifecycle = ok
                ? LibretroLifecycleState.GameLoaded
                : LibretroLifecycleState.Error;

            return ok;
        }

        public void StartEmulation()
        {
            if (!state.GameLoaded)
                return;

            state.Running = true;
            state.Lifecycle = LibretroLifecycleState.Running;
        }

        public void StopEmulation()
        {
            state.Running = false;
            state.Lifecycle = LibretroLifecycleState.Created;

            LibretroNative.retro_deinit();
            LibretroEnvironment.Cleanup();
        }

        public bool IsRunning => state != null && state.Running;

        // 🔑 EXECUÇÃO SEGURA: chamada pelo Manager no Update
        public bool RunFrame()
        {
            if (!IsRunning)
                return false;

            LibretroNative.retro_run();
            return true;
        }

        public Texture GetVideoTexture()
        {
            if (!frameReady || videoBuffer == null)
                return null;

            if (videoTexture == null ||
                videoTexture.width != videoWidth ||
                videoTexture.height != videoHeight)
            {
                videoTexture = new Texture2D(
                    videoWidth,
                    videoHeight,
                    TextureFormat.RGB565,
                    false
                );
                videoTexture.filterMode = FilterMode.Point;
            }

            videoTexture.LoadRawTextureData(videoBuffer);
            videoTexture.Apply(false);

            frameReady = false;
            return videoTexture;
        }

        // ⚠️ CALLBACK SIMPLES E SEGURO
        private void OnVideoRefresh(IntPtr data, uint w, uint h, ulong pitch)
        {
            if (data == IntPtr.Zero)
                return;

            int rowSize = (int)w * 2; // RGB565
            int size = rowSize * (int)h;

            if (videoBuffer == null || videoBuffer.Length != size)
                videoBuffer = new byte[size];

            for (int y = 0; y < h; y++)
            {
                IntPtr src = IntPtr.Add(data, y * (int)pitch);
                Marshal.Copy(src, videoBuffer, y * rowSize, rowSize);
            }

            videoWidth = (int)w;
            videoHeight = (int)h;
            frameReady = true;

            state.VideoWidth = videoWidth;
            state.VideoHeight = videoHeight;
            state.PixelFormat = "RGB565";
        }

        private UIntPtr OnAudioSampleBatch(IntPtr data, UIntPtr frames)
        {
            int frameCount = (int)frames;
            int samples = frameCount * 2; // estéreo: L + R

            if (audioBuffer == null || audioBuffer.Length < samples)
                audioBuffer = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                short sample = Marshal.ReadInt16(data, i * 2);
                audioBuffer[i] = sample / 32768f;
            }

            audioFrames = frameCount * 2; // samples reais
            audioReady = true;

            for (int i = 0; i < samples; i++)
            {
                short sample = Marshal.ReadInt16(data, i * 2);
                audioRing[audioWritePos] = sample / 32768f;
                audioWritePos = (audioWritePos + 1) % AUDIO_RING_SIZE;
            }

            return frames;
        }

        public bool TryGetAudio(out float[] buffer)
        {
            if (!audioReady)
            {
                buffer = null;
                return false;
            }

            buffer = audioBuffer;
            audioReady = false;
            return true;
        }

        public void ReadAudio(float[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (audioReadPos != audioWritePos)
                {
                    output[i] = audioRing[audioReadPos];
                    audioReadPos = (audioReadPos + 1) % AUDIO_RING_SIZE;
                }
                else
                {
                    output[i] = 0f;
                }
            }
        }

        public int GetSampleRate() => 0;
        public int GetAudioChannels() => 0;
        public float[] GetAudioBuffer() => null;
    }
}
