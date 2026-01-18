using System.Collections.Generic;
using UnityEngine;
using RetroLib.Engine.Core;
using RetroLib.LibretroHost.Core;
using RetroLib.Debugging;
using System;
using RetroLib.Infrastructure.FileSystem;
using RetroLib.Engine.Input;

namespace RetroLib.Engine.Manager
{
    public class RetroLibManager : MonoBehaviour
    {
        [Header("Video Output")]
        [SerializeField] private RenderTexture targetTexture;

        [Header("Audio Output")]
        [SerializeField] private AudioSource audioSource;

        public DebugStats DebugStats { get; private set; } = new DebugStats();

        private readonly List<RetroSession> sessions = new();
        private float deltaTime;

        // Timing
        private float emuAccumulator;
        private float emuDelta;

        private IRetroInputProvider inputProvider;

        private void Awake()
        {
            RetroFileSystem.Initialize();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
            audioSource.mute = false;

            // Clip dummy para manter pipeline ativo
            audioSource.clip = AudioClip.Create(
                "LibretroAudio",
                44100,
                2,
                44100,
                true
            );

            audioSource.Play();

            inputProvider = new KeyboardInputProvider();

            Debug.Log("[RetroLibManager] Runtime ready");
        }

        // =========================
        // SESSION MANAGEMENT
        // =========================

        public RetroSession CreateSession()
        {
            var core = new LibretroInstance();
            var session = new RetroSession(core);

            sessions.Add(session);
            return session;
        }

        public void DestroySession(RetroSession session)
        {
            if (sessions.Contains(session))
            {
                session.Stop();
                sessions.Remove(session);
            }
        }

        // =========================
        // UPDATE LOOP
        // =========================

        private void Update()
        {
            if (sessions.Count == 0)
                return;

            UpdatePerformanceStats();
            UpdateDebugStats();

            var session = sessions[0];
            var libretro = session.Core as LibretroInstance;
            var state = libretro?.GetState();

            if (state != null && state.TargetFps > 0f)
            {
                emuDelta = 1f / state.TargetFps;
            }
            else
            {
                emuDelta = 1f / 60f; // fallback seguro
            }

            emuAccumulator += Time.unscaledDeltaTime;

            inputProvider.Poll();

            while (emuAccumulator >= emuDelta)
            {
                foreach (var s in sessions)
                {
                    if (session.Core is LibretroInstance libretroInstance)
                    {
                        libretroInstance.SetInputState(inputProvider.State);
                    }

                    if (s.IsRunning)
                        s.RunFrame();
                }

                emuAccumulator -= emuDelta;
            }

            var video = sessions[0].Core.GetVideoTexture();
            if (video != null && targetTexture != null)
            {
                Graphics.Blit(video, targetTexture, new Vector2(1, -1), new Vector2(0, 1));
            }
        }


        // =========================
        // AUDIO (UNITY THREAD)
        // =========================

        private void OnAudioFilterRead(float[] data, int channels)
        {
            Array.Clear(data, 0, data.Length);

            if (sessions.Count == 0)
                return;

            var session = sessions[0];

            if (!session.IsRunning)
                return;

            session.Core.ReadAudio(data, data.Length);
        }

        // =========================
        // DEBUG
        // =========================

        private void UpdatePerformanceStats()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            DebugStats.fps = 1.0f / deltaTime;
            DebugStats.frameTimeMs = deltaTime * 1000.0f;
        }

        private void UpdateDebugStats()
        {
            DebugStats.activeSessions = sessions.Count;

            if (sessions.Count == 0)
                return;

            var session = sessions[0];
            var core = session.Core;

            if (core is not LibretroHost.Core.LibretroInstance libretro)
                return;

            var state = libretro.GetState(); // vamos expor isso já já

            DebugStats.coreRunning = state.Running;
            DebugStats.gameLoaded = state.GameLoaded;
            DebugStats.lifecycle = state.Lifecycle.ToString();

            DebugStats.coreName = state.CoreName;
            DebugStats.coreVersion = state.CoreVersion;
            DebugStats.systemName = state.SystemName;

            DebugStats.videoWidth = state.VideoWidth;
            DebugStats.videoHeight = state.VideoHeight;
            DebugStats.pixelFormat = state.PixelFormat;

            DebugStats.audioSampleRate = state.AudioSampleRate;
            DebugStats.audioActive = state.Running;

            DebugStats.needsFullPath = state.NeedsFullPath;
        }

    }
}
