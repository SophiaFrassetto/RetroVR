using UnityEngine;

namespace RetroLib.Debugging
{
    [System.Serializable]
    public class DebugStats
    {
        // Performance
        public float fps;
        public float frameTimeMs;

        // Core
        public bool coreRunning;
        public bool gameLoaded;
        public string coreName;
        public string coreVersion;
        public string systemName;

        // Video
        public int videoWidth;
        public int videoHeight;
        public string pixelFormat;

        // Audio
        public int audioSampleRate;

        // Paths / Flags
        public bool needsFullPath;

        public void Reset()
        {
            fps = 0;
            frameTimeMs = 0;

            coreRunning = false;
            gameLoaded = false;
            coreName = "None";
            coreVersion = "Unknown";
            systemName = "Unknown";

            videoWidth = 0;
            videoHeight = 0;
            pixelFormat = "Unknown";

            audioSampleRate = 0;
            needsFullPath = false;
        }
    }
}
