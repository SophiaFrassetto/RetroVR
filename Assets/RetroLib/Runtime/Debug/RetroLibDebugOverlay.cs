using UnityEngine;

namespace RetroLib.Debugging
{
    public class RetroLibDebugOverlay : DebugOverlay
    {
        protected override void DrawCustomStats()
        {
            GUILayout.Space(10);
            GUILayout.Label("Core");

            GUILayout.Label($"Running: {DebugStats.CoreRunning}");
            GUILayout.Label($"Core Name: {DebugStats.CoreName}");
            GUILayout.Label($"Video: {DebugStats.VideoWidth} x {DebugStats.VideoHeight}");
            GUILayout.Label($"Audio Rate: {DebugStats.AudioSampleRate}");
        }
    }
}
