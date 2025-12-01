using System;
using UnityEngine;

namespace retrovr.system
{
    public class ScreenInstance : MonoBehaviour
    {
        #region Fields
        [Header("Screen Configuration")]
        [SerializeField] public string screenName;

        [Header("State")]
        [SerializeField] private ScreenOperationalState operationalState = ScreenOperationalState.Off;
        [SerializeField] private ScreenPhysicalState physicalState = ScreenPhysicalState.Loose;

        public event Action<ScreenOperationalState> OnOperationalStateChanged;
        public event Action<ScreenPhysicalState> OnPhysicalStateChanged;

        [Header("Renderer / Material")]
        [SerializeField] public Renderer screenRenderer; // assign in prefab
        [SerializeField] public Collider screenCollider; // assign in prefab
        [SerializeField] private Material noSignalMaterial; // PNG / texture for NoSignal
        [SerializeField] private Texture staticTexture; // optional GIF-like texture sheet or animated material

        [Header("Audio")]
        [SerializeField] private AudioSource tvAudioSource;
        [SerializeField] private AudioClip staticLoopClip;
        [SerializeField] private AudioClip clickClip;
        #endregion

        #region Screen Power Management
        #endregion

        #region Screen Volume Management
        #endregion

        #region State Management
        public void SetOperationalState(ScreenOperationalState newState)
        {
            if (operationalState == newState) return;
            operationalState = newState;
            Log.Info($"[ScreenInstance] OperationalState -> {operationalState}");
            OnOperationalStateChanged?.Invoke(newState);
            UpdateVisualsForState(newState);
        }

        public void SetPhysicalState(ScreenPhysicalState newState)
        {
            if (physicalState == newState) return;
            physicalState = newState;
            Log.Info($"[ScreenInstance] PhysicalState -> {physicalState}");
            OnPhysicalStateChanged?.Invoke(newState);
            // small local effects: enable collisions, mount transforms, etc.
        }

        private void UpdateVisualsForState(ScreenOperationalState state)
        {
            switch (state)
            {
                case ScreenOperationalState.Off:
                    if (tvAudioSource != null) tvAudioSource.Stop();
                    ApplyNoSignal(false);
                    SetRendererEnabled(false);
                    break;

                case ScreenOperationalState.Standby:
                case ScreenOperationalState.NoSignal:
                    SetRendererEnabled(true);
                    ApplyNoSignal(true);
                    PlayStaticLoop(false); // maybe no audio for NoSignal if prefer only image
                    break;

                case ScreenOperationalState.Booting:
                    SetRendererEnabled(true);
                    ApplyStaticTexture();
                    PlayStaticLoop(true);
                    break;

                case ScreenOperationalState.ShowingContent:
                    // actual content rendering will be provided by emulator libretro into a RenderTexture
                    // ensure tvAudioSource stops the static
                    PlayStaticLoop(false);
                    // material should display the RenderTexture assigned by emulator
                    break;

                case ScreenOperationalState.Error:
                    SetRendererEnabled(true);
                    ApplyNoSignal(true);
                    PlayStaticLoop(true);
                    break;
            }
        }

        private void SetRendererEnabled(bool enabled)
        {
            if (screenRenderer == null) return;
            screenRenderer.enabled = enabled;
        }

        private void ApplyNoSignal(bool show)
        {
            if (screenRenderer == null || noSignalMaterial == null) return;
            if (show)
                screenRenderer.material = noSignalMaterial;
        }

        private void ApplyStaticTexture()
        {
            if (screenRenderer == null || staticTexture == null) return;
            screenRenderer.material.mainTexture = staticTexture;
            // if you want animated static, use a shader that scrolls uv or a flipbook shader
        }

        private void PlayStaticLoop(bool play)
        {
            if (tvAudioSource == null || staticLoopClip == null) return;
            if (play)
            {
                tvAudioSource.loop = true;
                if (!tvAudioSource.isPlaying) tvAudioSource.Play();
            }
            else
            {
                if (tvAudioSource.isPlaying) tvAudioSource.Stop();
            }
        }
        #endregion
    }
}
