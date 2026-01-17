using UnityEngine;
using System.Text;

namespace RetroLib.Debugging
{
    public class DebugOverlay : MonoBehaviour
    {
        public bool showOverlay = true;
        public KeyCode toggleKey = KeyCode.F1;

        private float deltaTime;

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                showOverlay = !showOverlay;

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!showOverlay)
                return;

            float fps = 1.0f / deltaTime;

            GUILayout.BeginArea(new Rect(10, 10, 400, 400), GUI.skin.box);
            GUILayout.Label("RetroLib Debug");
            GUILayout.Space(5);

            GUILayout.Label($"FPS: {fps:0.}");
            GUILayout.Label($"Frame Time: {deltaTime * 1000f:0.0} ms");

            DrawCustomStats();

            GUILayout.EndArea();
        }

        protected virtual void DrawCustomStats()
        {
            // extensível
        }
    }
}
