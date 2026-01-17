using UnityEngine;
using RetroLib.Core;
using RetroLib.Debugging;

namespace RetroLib.Manager
{
    /// <summary>
    /// Responsável por coordenar vídeo, áudio e ciclo de vida do core.
    /// Não contém lógica específica de Libretro.
    /// </summary>
    public class RetroLibManager : MonoBehaviour
    {
        [Header("Video Output")]
        [SerializeField] private RenderTexture targetTexture;

        [Header("Audio Output")]
        [SerializeField] private AudioSource audioSource;

        private IRetroCore core;

        void Awake()
        {
            Debug.Log("[RetroLibManager] Awake");
        }

        void Update()
        {
            if (core == null)
            {
                DebugStats.CoreRunning = false;
                return;
            }

            DebugStats.CoreRunning = core.IsRunning;
            DebugStats.CoreName = core.GetType().Name;

            // var video = core.GetVideoTexture();
            // if (video != null && targetTexture != null)
            // {
            //     DebugStats.VideoWidth = video.width;
            //     DebugStats.VideoHeight = video.height;

            //     Graphics.Blit(video, targetTexture);
            // }

            DebugStats.AudioSampleRate = core.GetSampleRate();
        }

        public void SetCore(IRetroCore newCore)
        {
            core = newCore;
        }
    }
}
