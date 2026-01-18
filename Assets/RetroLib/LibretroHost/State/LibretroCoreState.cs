namespace RetroLib.LibretroHost.State
{
    public enum LibretroLifecycleState
    {
        Created,
        EnvironmentReady,
        CoreInitialized,
        GameLoaded,
        Running,
        Error
    }

    public class LibretroCoreState
    {
        public readonly object Sync = new();

        public LibretroLifecycleState Lifecycle { get; set; } =
            LibretroLifecycleState.Created;

        public bool Running { get; set; }
        public bool GameLoaded { get; set; }
        public bool NeedsFullPath { get; set; }

        public string CoreName { get; set; }
        public string CoreVersion { get; set; }
        public string SystemName { get; set; }
        public string PixelFormat { get; set; }
        public string Extensions { get; set; }
        public float TargetFps { get; set; }

        public int VideoWidth { get; set; }
        public int VideoHeight { get; set; }
        public int AudioSampleRate { get; set; }
    }
}
