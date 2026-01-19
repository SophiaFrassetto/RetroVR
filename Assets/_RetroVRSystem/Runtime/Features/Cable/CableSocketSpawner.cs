using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using retrovr.system.cable;

namespace retrovr.system
{
    public class CableSocketSpawner : MonoBehaviour
    {
        public CableRuntime cablePrefab;
        public Transform attachPoint;

        private CableRuntime activeCable;
        private bool isConnected;

        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (activeCable != null) return;

            activeCable = Instantiate(cablePrefab);
            activeCable.Spawn(attachPoint, args.interactorObject.transform);
            isConnected = false;
        }

        public void OnSelectExited(SelectExitEventArgs args)
        {
            if (activeCable == null) return;

            if (!isConnected)
            {
                activeCable.DestroyCable();
                activeCable = null;
            }
        }

        public void NotifyConnected()
        {
            isConnected = true;
        }

        public void NotifyDisconnected()
        {
            isConnected = false;

            if (activeCable != null)
            {
                activeCable.DestroyCable();
                activeCable = null;
            }
        }
    }
}
