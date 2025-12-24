using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace retrovr.system
{
    public class CableSocket : MonoBehaviour
    {
        public GameObject videoInstance;

        public void OnPlugged(SelectEnterEventArgs args)
        {
            var plug = args.interactableObject.transform.GetComponent<CablePlug>();
            if (plug == null) return;
            plug.Connect(this);
        }

        public void OnUnplugged(SelectExitEventArgs args)
        {
            var plug = args.interactableObject.transform.GetComponent<CablePlug>();
            if (plug == null) return;
            plug.Disconnect();
        }
    }
}
