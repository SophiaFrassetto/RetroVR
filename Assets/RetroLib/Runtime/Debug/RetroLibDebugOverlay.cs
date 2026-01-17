using UnityEngine;
using RetroLib.Manager;

namespace RetroLib.Debugging
{
    public class RetroLibDebugOverlay : DebugOverlay
    {
        private GUIStyle style;

        private void Awake()
        {
            style = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.white }
            };
        }

        protected override void DrawOverlay()
        {
            var manager = RetroLibManager.Instance;
            if (manager == null)
                return;

            var stats = manager.DebugStats;
            if (stats == null)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 350, 500), GUI.skin.box);

            GUILayout.Label("RetroLib Debug", style);
            GUILayout.Space(5);

            GUILayout.Label($"FPS: {stats.fps:F0}", style);
            GUILayout.Label($"Frame Time: {stats.frameTimeMs:F1} ms", style);

            GUILayout.Space(10);
            GUILayout.Label("Core", style);

            GUILayout.Label($"Running: {stats.coreRunning}", style);
            GUILayout.Label($"Game Loaded: {stats.gameLoaded}", style);
            GUILayout.Label($"Core Name: {stats.coreName}", style);
            GUILayout.Label($"Core Version: {stats.coreVersion}", style);
            GUILayout.Label($"System: {stats.systemName}", style);

            GUILayout.Space(10);
            GUILayout.Label("Video", style);

            GUILayout.Label($"Resolution: {stats.videoWidth} x {stats.videoHeight}", style);
            GUILayout.Label($"Pixel Format: {stats.pixelFormat}", style);

            GUILayout.Space(10);
            GUILayout.Label("Audio", style);

            GUILayout.Label($"Sample Rate: {stats.audioSampleRate}", style);

            GUILayout.Space(10);
            GUILayout.Label("Flags", style);

            GUILayout.Label($"Needs Full Path: {stats.needsFullPath}", style);

            GUILayout.EndArea();
        }
    }
}
