using System;
using System.Runtime.InteropServices;
using UnityEngine;
using RetroLib.Engine.Audio;
using RetroLib.LibretroHost.Native;
using RetroLib.LibretroHost.State;
using RetroLib.Engine.Core;
using RetroLib.LibretroHost.Environment;
using RetroLib.Engine.Input;

namespace RetroLib.LibretroHost.Core
{
    public class LibretroInstance : IRetroCore
    {
        private LibretroCoreState state;
        private LibretroLoader loader;
        private LibretroApi api;

        // VIDEO
        private byte[] videoBuffer;
        private int videoWidth, videoHeight;
        private bool frameReady;
        private Texture2D videoTexture;

        // AUDIO
        private const int AUDIO_CAPACITY = 44100 * 2;
        private AudioRingBuffer audioRing;
        private float[] tempAudio;

        // Callbacks
        private LibretroApi.VideoRefreshCallback videoCb;
        private LibretroApi.AudioSampleBatchCallback audioBatchCb;
        private LibretroApi.InputPollCallback inputPollCb;
        private LibretroApi.InputStateCallback inputStateCb;

        // Input
        private RetroInputState inputState;

        public void SetInputState(RetroInputState state)
        {
            inputState = state;
        }

        public void SetState(LibretroCoreState coreState)
        {
            state = coreState;
            audioRing = new AudioRingBuffer(AUDIO_CAPACITY);
        }

        public bool LoadCore(string corePath)
        {
            loader = new LibretroLoader();
            loader.Load(corePath);

            api = new LibretroApi
            {
                retro_init = loader.LoadFunction<LibretroApi.retro_init_t>("retro_init"),
                retro_deinit = loader.LoadFunction<LibretroApi.retro_deinit_t>("retro_deinit"),
                retro_load_game = loader.LoadFunction<LibretroApi.retro_load_game_t>("retro_load_game"),
                retro_run = loader.LoadFunction<LibretroApi.retro_run_t>("retro_run"),

                retro_get_system_info =
                    loader.LoadFunction<LibretroApi.retro_get_system_info_t>("retro_get_system_info"),

                retro_set_environment =
                    loader.LoadFunction<LibretroApi.retro_set_environment_t>("retro_set_environment"),
                retro_set_video_refresh =
                    loader.LoadFunction<LibretroApi.retro_set_video_refresh_t>("retro_set_video_refresh"),
                retro_set_audio_sample_batch =
                    loader.LoadFunction<LibretroApi.retro_set_audio_sample_batch_t>("retro_set_audio_sample_batch"),
                retro_set_input_poll =
                    loader.LoadFunction<LibretroApi.retro_set_input_poll_t>("retro_set_input_poll"),
                retro_set_input_state =
                    loader.LoadFunction<LibretroApi.retro_set_input_state_t>("retro_set_input_state"),
            };

            // 🔑 ESTADO PRIMEIRO
            LibretroEnvironment.BindState(state);

            api.retro_set_environment(LibretroEnvironment.OnEnvironment);

            // callbacks
            videoCb = OnVideoRefresh;
            audioBatchCb = OnAudioSampleBatch;
            inputPollCb = () => { };
            inputStateCb = (port, device, index, id) =>
            {
                if (device != LibretroConstants.RETRO_DEVICE_JOYPAD || inputState == null)
                    return 0;

                return inputState.GetButton((int)port, (int)id) ? (short)1 : (short)0;
            };
            api.retro_set_video_refresh(videoCb);
            api.retro_set_audio_sample_batch(audioBatchCb);
            api.retro_set_input_poll(inputPollCb);
            api.retro_set_input_state(inputStateCb);

            api.retro_init();

            // ================= SYSTEM INFO =================

            if (api.retro_get_system_info != null)
            {
                LibretroApi.retro_system_info sysInfo;
                api.retro_get_system_info(out sysInfo);

                state.CoreName = Marshal.PtrToStringAnsi(sysInfo.library_name);
                state.CoreVersion = Marshal.PtrToStringAnsi(sysInfo.library_version);
                state.SystemName = Marshal.PtrToStringAnsi(sysInfo.valid_extensions);
                state.NeedsFullPath = sysInfo.need_fullpath;
            }

            return true;
        }

        public bool LoadRom(string romPath)
        {
            var game = new LibretroApi.retro_game_info
            {
                path = Marshal.StringToHGlobalAnsi(romPath)
            };

            bool ok = api.retro_load_game(ref game);
            Marshal.FreeHGlobal(game.path);

            state.GameLoaded = ok;
            return ok;
        }

        public void StartEmulation() => state.Running = true;

        public void StopEmulation()
        {
            state.Running = false;
            audioRing.Clear();
            api?.retro_deinit();
            loader?.Unload();
        }

        public bool IsRunning => state != null && state.Running;

        public bool RunFrame()
        {
            if (!IsRunning) return false;
            api.retro_run();
            return true;
        }

        // VIDEO
        public Texture GetVideoTexture()
        {
            if (!frameReady || videoBuffer == null) return null;

            if (videoTexture == null || videoTexture.width != videoWidth || videoTexture.height != videoHeight)
            {
                videoTexture = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB565, false)
                {
                    filterMode = FilterMode.Point
                };
            }

            videoTexture.LoadRawTextureData(videoBuffer);
            videoTexture.Apply(false);
            frameReady = false;
            return videoTexture;
        }

        private void OnVideoRefresh(IntPtr data, uint w, uint h, ulong pitch)
        {
            int rowSize = (int)w * 2;
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
        }

        // AUDIO
        private UIntPtr OnAudioSampleBatch(IntPtr data, UIntPtr frames)
        {
            int samples = (int)frames * 2;

            if (tempAudio == null || tempAudio.Length < samples)
                tempAudio = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                short s = Marshal.ReadInt16(data, i * 2);
                tempAudio[i] = s / 32768f;
            }

            audioRing.Write(tempAudio, samples);
            return frames;
        }

        public void ReadAudio(float[] output, int length)
        {
            if (!IsRunning || audioRing == null)
            {
                // silêncio
                for (int i = 0; i < length; i++)
                    output[i] = 0f;
                return;
            }

            int read = audioRing.Read(output, length);

            for (int i = read; i < length; i++)
                output[i] = 0f;
        }

        public LibretroCoreState GetState()
        {
            return state;
        }

        // Unused for now
        public int GetSampleRate() => state.AudioSampleRate;
        public int GetAudioChannels() => 2;
        public float[] GetAudioBuffer() => null;
        public bool TryGetAudio(out float[] buffer) { buffer = null; return false; }
    }
}
