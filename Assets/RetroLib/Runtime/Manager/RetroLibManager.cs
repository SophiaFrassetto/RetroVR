using UnityEngine;
using RetroLib.Core;
using RetroLib.Debugging;

namespace RetroLib.Manager
{
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

        public void SetCore(IRetroCore newCore)
        {
            core = newCore;
            DebugStats.CoreName = core.GetType().Name;
        }

        void Update()
        {
            if (core == null || !core.IsRunning)
                return;

            // 🔑 EXECUTA UM FRAME (CONTROLADO)
            core.RunFrame();

            // 🎥 ATUALIZA VÍDEO
            var video = core.GetVideoTexture();
            if (video != null && targetTexture != null)
            {
                Graphics.Blit(video, targetTexture);
            }
        }
    }
}
