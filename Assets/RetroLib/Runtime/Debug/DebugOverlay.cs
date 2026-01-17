using UnityEngine;

namespace RetroLib.Debugging
{
    public abstract class DebugOverlay : MonoBehaviour
    {
        public bool ShowOverlay = true;
        public KeyCode ToggleKey = KeyCode.F1;

        protected virtual void Update()
        {
            if (Input.GetKeyDown(ToggleKey))
                ShowOverlay = !ShowOverlay;
        }

        protected abstract void DrawOverlay();

        protected virtual void OnGUI()
        {
            if (!ShowOverlay)
                return;

            DrawOverlay();
        }
    }
}
