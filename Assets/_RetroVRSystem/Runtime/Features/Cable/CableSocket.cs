using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    public class CableSocket : MonoBehaviour
    {
        public GameObject videoInstance;

        public void OnPlugged(SelectEnterEventArgs args)
        {
            GetComponent<CableSocketSpawner>()?.NotifyConnected();
        }

        public void OnUnplugged(SelectExitEventArgs args)
        {
            GetComponent<CableSocketSpawner>()?.NotifyDisconnected();
        }
    }
}
