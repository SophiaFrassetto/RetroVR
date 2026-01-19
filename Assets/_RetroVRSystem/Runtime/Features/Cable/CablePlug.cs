using UnityEngine;

namespace retrovr.system
{
    public class CablePlug : MonoBehaviour
    {
        public CableSocket connectedSocket;

        public void Connect(CableSocket socket)
        {
            connectedSocket = socket;

            var phys = GetComponent<retrovr.system.physics.XRPhysicalObject>();
            phys?.ForceKinematic();
        }

        public void Disconnect()
        {
            connectedSocket = null;

            var phys = GetComponent<retrovr.system.physics.XRPhysicalObject>();
            phys?.OnReleased();
        }
    }
}
