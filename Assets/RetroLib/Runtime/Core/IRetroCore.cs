using UnityEngine;

namespace RetroLib.Core
{
    /// <summary>
    /// Interface base para qualquer implementação de core (Libretro ou mock).
    /// Esta interface NÃO conhece Unity VR, física ou cena.
    /// </summary>
    public interface IRetroCore
    {
        // Lifecycle
        bool LoadCore(string corePath);
        bool LoadRom(string romPath);

        void StartEmulation();
        void StopEmulation();

        bool IsRunning { get; }

        // Video
        Texture GetVideoTexture();

        // Audio (placeholder por enquanto)
        int GetSampleRate();
        int GetAudioChannels();
        float[] GetAudioBuffer();
    }
}
