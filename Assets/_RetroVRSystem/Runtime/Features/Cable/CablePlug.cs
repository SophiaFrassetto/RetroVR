using UnityEngine;

namespace retrovr.system
{
    public class CablePlug : MonoBehaviour
    {
        public CableSocket connectedSocket;

        public void Connect(CableSocket socket)
        {
            connectedSocket = socket;
        }

        public void Disconnect()
        {
            connectedSocket = null;
        }
    }
}
