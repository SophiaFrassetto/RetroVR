using RetroLib.Libretro;
using UnityEngine;

namespace RetroLib.Core
{
    public interface IRetroCore
    {
        // Lifecycle
        bool LoadCore(string corePath);
        bool LoadRom(string romPath);

        void StartEmulation();
        void StopEmulation();
        void SetState(LibretroCoreState coreState);

        bool IsRunning { get; }

        // Execution
        bool RunFrame();

        // Video
        Texture GetVideoTexture();

        // Audio (placeholder)
        int GetSampleRate();
        int GetAudioChannels();
        float[] GetAudioBuffer();
        bool TryGetAudio(out float[] buffer);
        void ReadAudio(float[] output, int length);
    }
}
