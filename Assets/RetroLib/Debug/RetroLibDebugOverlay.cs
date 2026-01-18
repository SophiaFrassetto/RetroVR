using UnityEngine;
using RetroLib.Engine.Manager;

namespace RetroLib.Debugging
{
    public class RetroLibDebugOverlay : DebugOverlay
    {
        [SerializeField] private RetroLibManager runtime;

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
            if (runtime == null)
            {
                GUILayout.Label("RetroLib Runtime: NOT FOUND");
                return;
            }

            var s = runtime.DebugStats;

            GUILayout.BeginArea(new Rect(10, 10, 380, 520), GUI.skin.box);

            GUILayout.Label("RetroLib Debug", style);
            GUILayout.Space(5);

            // ===== Runtime =====
            GUILayout.Label($"FPS: {s.fps:F0}", style);
            GUILayout.Label($"Frame Time: {s.frameTimeMs:F1} ms", style);
            GUILayout.Label($"Sessions: {s.activeSessions}", style);

            GUILayout.Space(10);

            // ===== Core =====
            GUILayout.Label("Core", style);
            GUILayout.Label($"Running: {s.coreRunning}", style);
            GUILayout.Label($"Game Loaded: {s.gameLoaded}", style);
            GUILayout.Label($"Lifecycle: {s.lifecycle}", style);
            GUILayout.Label($"Core: {s.coreName} {s.coreVersion}", style);
            GUILayout.Label($"System: {s.systemName}", style);

            GUILayout.Space(10);

            // ===== Video =====
            GUILayout.Label("Video", style);
            GUILayout.Label($"Resolution: {s.videoWidth} x {s.videoHeight}", style);
            GUILayout.Label($"Pixel Format: {s.pixelFormat}", style);

            GUILayout.Space(10);

            // ===== Audio =====
            GUILayout.Label("Audio", style);
            GUILayout.Label($"Sample Rate: {s.audioSampleRate}", style);
            GUILayout.Label($"Active: {s.audioActive}", style);

            GUILayout.Space(10);

            // ===== Flags =====
            GUILayout.Label("Flags", style);
            GUILayout.Label($"Needs Full Path: {s.needsFullPath}", style);

            GUILayout.EndArea();
        }
    }
}
