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

        bool IsRunning { get; }

        // Execution
        bool RunFrame();

        // Video
        Texture GetVideoTexture();

        // Audio (placeholder)
        int GetSampleRate();
        int GetAudioChannels();
        float[] GetAudioBuffer();
    }
}
