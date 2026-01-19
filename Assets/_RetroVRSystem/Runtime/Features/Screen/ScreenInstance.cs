using System;
using UnityEngine;

namespace retrovr.system
{
    public class ScreenInstance : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private bool isPoweredOn = false;
        [SerializeField] private bool hasSignal = false;

        [SerializeField] private ScreenOperationalState operationalState = ScreenOperationalState.Off;
        [SerializeField] private ScreenPhysicalState physicalState = ScreenPhysicalState.Loose;

        public event Action<ScreenOperationalState> OnOperationalStateChanged;
        public event Action<ScreenPhysicalState> OnPhysicalStateChanged;

        [Header("Renderer")]
        [SerializeField] public Renderer screenRenderer;
        [SerializeField] public Collider screenCollider;
        [SerializeField] private Material noSignalMaterial;
        [SerializeField] private Texture staticTexture;

        [Header("Audio")]
        [SerializeField] private bool playAudio = true;
        [SerializeField] private AudioSource feedbackAudioSource;
        [SerializeField] private AudioSource tvAudioSource;
        [SerializeField] private AudioClip staticLoopClip;
        [SerializeField] private AudioClip clipButtom;

        private Material runtimeMaterial;

        void Awake()
        {
            if (screenRenderer != null && noSignalMaterial != null)
            {
                runtimeMaterial = new Material(noSignalMaterial);
                screenRenderer.material = runtimeMaterial;
            }

            ApplyState();
        }

        // =====================
        // POWER
        // =====================

        public void PowerOn()
        {
            TryPlayOneShot(clipButtom);
            isPoweredOn = true;
            ApplyState();
        }

        public void PowerOff()
        {
            TryPlayOneShot(clipButtom);
            isPoweredOn = false;
            hasSignal = false;
            SetOperationalState(ScreenOperationalState.Off);
        }

        public void Power()
        {
            if (isPoweredOn)
                PowerOff();
            else
                PowerOn();
        }

        // =====================
        // SIGNAL / CONSOLE
        // =====================

        public void SetSignalPresent(bool present)
        {
            hasSignal = present;
            ApplyState();
        }

        public void SetConsoleRunning(bool running)
        {
            if (!isPoweredOn) return;

            if (!hasSignal)
            {
                SetOperationalState(ScreenOperationalState.NoSignal);
            }
            else if (running)
            {
                SetOperationalState(ScreenOperationalState.ShowingContent);
            }
            else
            {
                SetOperationalState(ScreenOperationalState.Booting); // static
            }
        }

        // =====================
        // STATE HANDLING
        // =====================

        private void ApplyState()
        {
            if (!isPoweredOn)
            {
                SetOperationalState(ScreenOperationalState.Off);
                return;
            }

            if (!hasSignal)
            {
                SetOperationalState(ScreenOperationalState.NoSignal);
                return;
            }

            // powered + signal but not running yet
            SetOperationalState(ScreenOperationalState.Booting);
        }

        public void SetOperationalState(ScreenOperationalState newState)
        {
            if (operationalState == newState) return;
            operationalState = newState;

            UpdateVisuals();
            OnOperationalStateChanged?.Invoke(newState);
        }

        public void SetPhysicalState(ScreenPhysicalState newState)
        {
            if (physicalState == newState) return;
            physicalState = newState;

            var phys = GetComponent<physics.XRPhysicalObject>();
            if (phys != null)
            {
                if (newState == ScreenPhysicalState.Held)
                    phys.OnGrabbed();
                else if (newState == ScreenPhysicalState.Mounted)
                    phys.ForceKinematic();
                else
                    phys.OnReleased();
            }

            OnPhysicalStateChanged?.Invoke(newState);
        }

        // =====================
        // VISUALS / AUDIO
        // =====================

        private void UpdateVisuals()
        {
            if (screenRenderer == null || runtimeMaterial == null) return;

            switch (operationalState)
            {
                case ScreenOperationalState.Off:
                    screenRenderer.enabled = false;
                    StopStatic();
                    break;

                case ScreenOperationalState.NoSignal:
                    screenRenderer.enabled = true;
                    runtimeMaterial.mainTexture = null;
                    StopStatic();
                    break;

                case ScreenOperationalState.Booting:
                    screenRenderer.enabled = true;
                    runtimeMaterial.mainTexture = staticTexture;
                    PlayStatic();
                    break;

                case ScreenOperationalState.ShowingContent:
                    screenRenderer.enabled = true;
                    StopStatic();
                    break;

                case ScreenOperationalState.Error:
                    screenRenderer.enabled = true;
                    runtimeMaterial.mainTexture = null;
                    PlayStatic();
                    break;
            }
        }

        private void PlayStatic()
        {
            if (tvAudioSource == null || staticLoopClip == null) return;
            if (!tvAudioSource.isPlaying)
            {
                tvAudioSource.clip = staticLoopClip;
                tvAudioSource.loop = true;
                tvAudioSource.Play();
            }
        }

        private void StopStatic()
        {
            if (tvAudioSource != null && tvAudioSource.isPlaying)
                tvAudioSource.Stop();
        }

        // =====================
        // VOLUME
        // =====================

        public void VolumeUp()
        {
            if (tvAudioSource == null) return;
            TryPlayOneShot(clipButtom);
            tvAudioSource.volume = Mathf.Clamp01(tvAudioSource.volume + 0.1f);
        }

        public void VolumeDown()
        {
            if (tvAudioSource == null) return;
            TryPlayOneShot(clipButtom);
            tvAudioSource.volume = Mathf.Clamp01(tvAudioSource.volume - 0.1f);
        }

        private void TryPlayOneShot(AudioClip clip)
        {
            if (!playAudio) return;
            if (clip == null || feedbackAudioSource == null) return;
            feedbackAudioSource.PlayOneShot(clip);
        }
    }
}
