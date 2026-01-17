using UnityEngine;

namespace RetroLib.Core
{
    /// <summary>
    /// Core falso para testar pipeline de vídeo sem Libretro.
    /// Gera uma textura simples animada.
    /// </summary>
    public class MockRetroCore : IRetroCore
    {
        private Texture2D texture;
        private bool running;
        private float time;

        public bool LoadCore(string corePath)
        {
            return true;
        }

        public bool LoadRom(string romPath)
        {
            return true;
        }

        public void StartEmulation()
        {
            texture = new Texture2D(256, 224, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            running = true;
        }

        public void StopEmulation()
        {
            running = false;
        }

        public bool IsRunning => running;

        // 🔑 NOVO MÉTODO (OBRIGATÓRIO AGORA)
        public bool RunFrame()
        {
            if (!running || texture == null)
                return false;

            time += Time.deltaTime;
            return true;
        }

        public Texture GetVideoTexture()
        {
            if (!running || texture == null)
                return null;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float v = Mathf.PingPong(time + (x * 0.01f), 1f);
                    texture.SetPixel(x, y, new Color(v, 0.2f, 0.8f));
                }
            }

            texture.Apply();
            return texture;
        }

        public int GetSampleRate() => 44100;
        public int GetAudioChannels() => 2;
        public float[] GetAudioBuffer() => null;
    }
}
