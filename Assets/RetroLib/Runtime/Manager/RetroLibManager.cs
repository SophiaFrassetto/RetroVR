using UnityEngine;
using RetroLib.Core;
using RetroLib.Debugging;
using RetroLib.Libretro;
using System;

namespace RetroLib.Manager
{
    public class RetroLibManager : MonoBehaviour
    {
        [Header("Video Output")]
        [SerializeField] private RenderTexture targetTexture;

        [Header("Audio Output")]
        [SerializeField] private AudioSource audioSource;

        public static RetroLibManager Instance { get; private set; }

        public DebugStats DebugStats { get; private set; } = new DebugStats();
        private float deltaTime;

        public IRetroCore core;

        private LibretroCoreState coreState;

        private float emuAccumulator;
        private const float EmuDelta = 1f / 60f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

            coreState = new LibretroCoreState();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f; // 2D
            audioSource.volume = 1f;
            audioSource.mute = false;

            // força o AudioSource a "existir" no pipeline
            audioSource.clip = AudioClip.Create(
                "LibretroAudio",
                44100,      // tamanho dummy
                2,          // estéreo
                44100,
                true
            );

            audioSource.Play();

            Debug.Log("[RetroLibManager] Awake");
        }

        public void CreateCore()
        {
            if (core != null)
                return;

            core = new RetroCoreLibretro();
            core.SetState(coreState);
        }

        public void SetCore(IRetroCore retroCore)
        {
            core = retroCore;
            core.SetState(coreState);
        }

        void Update()
        {
            if (core == null || !core.IsRunning)
                return;

            UpdatePerformanceStats();
            UpdateCoreStats();

            emuAccumulator += Time.unscaledDeltaTime;

            while (emuAccumulator >= EmuDelta)
            {
                core.RunFrame();
                emuAccumulator -= EmuDelta;
            }

            var video = core.GetVideoTexture();
            if (video != null && targetTexture != null)
            {
                Graphics.Blit(
                    video,
                    targetTexture,
                    new Vector2(1, -1),   // escala
                    new Vector2(0, 1)     // offset
                );
            }
        }

        private void UpdatePerformanceStats()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            DebugStats.fps = 1.0f / deltaTime;
            DebugStats.frameTimeMs = deltaTime * 1000.0f;
        }

        private void UpdateCoreStats()
        {
            if (coreState == null)
                return;

            DebugStats.coreRunning = core.IsRunning;
            DebugStats.gameLoaded = coreState.GameLoaded;

            DebugStats.coreName = coreState.CoreName;
            DebugStats.coreVersion = coreState.CoreVersion;
            DebugStats.systemName = coreState.SystemName;

            DebugStats.videoWidth = coreState.VideoWidth;
            DebugStats.videoHeight = coreState.VideoHeight;
            DebugStats.pixelFormat = coreState.PixelFormat;

            DebugStats.audioSampleRate = coreState.AudioSampleRate;
            DebugStats.needsFullPath = coreState.NeedsFullPath;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            Array.Clear(data, 0, data.Length);

            if (core == null)
                return;

            core.ReadAudio(data, data.Length);
        }
    }
}
