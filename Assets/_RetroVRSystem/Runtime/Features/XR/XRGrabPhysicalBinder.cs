using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system.physics
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class XRGrabPhysicalBinder : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
        private XRPhysicalObject physical;

        void Awake()
        {
            grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            physical = GetComponent<XRPhysicalObject>();

            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }

        void OnDestroy()
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }

        void OnGrab(SelectEnterEventArgs args)
        {
            physical?.OnGrabbed();
        }

        void OnRelease(SelectExitEventArgs args)
        {
            physical?.OnReleased();
        }
    }
}
