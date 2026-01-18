using System;

namespace RetroLib.Debugging
{
    [Serializable]
    public class DebugStats
    {
        // ================= RUNTIME =================
        public float fps;
        public float frameTimeMs;
        public int activeSessions;

        // ================= CORE =================
        public bool coreRunning;
        public bool gameLoaded;
        public string lifecycle;

        public string coreName;
        public string coreVersion;
        public string systemName;

        // ================= VIDEO =================
        public int videoWidth;
        public int videoHeight;
        public string pixelFormat;

        // ================= AUDIO =================
        public int audioSampleRate;
        public bool audioActive;

        // ================= FLAGS =================
        public bool needsFullPath;
    }
}
