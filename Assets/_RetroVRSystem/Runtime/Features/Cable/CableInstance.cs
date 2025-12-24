using UnityEngine;

namespace retrovr.system
{
    public class CableInstance : MonoBehaviour
    {
        public CablePlug A;
        public CablePlug B;
        public LineRenderer line;

        public bool IsFullyConnected =>
            A.connectedSocket != null &&
            B.connectedSocket != null;

        private ConsoleInstance currentConsole;
        private ScreenInstance currentScreen;

        void Update()
        {
            line.SetPosition(0, A.transform.position);
            line.SetPosition(1, B.transform.position);

            if (A.connectedSocket != null && B.connectedSocket != null)
            {
                GameObject AVideoInstance = A.connectedSocket.videoInstance;
                GameObject BVideoInstance = B.connectedSocket.videoInstance;

                var console = AVideoInstance.GetComponent<ConsoleInstance>() ?? BVideoInstance.GetComponent<ConsoleInstance>();
                var screen  = AVideoInstance.GetComponent<ScreenInstance>()  ?? BVideoInstance.GetComponent<ScreenInstance>();

                if (console != currentConsole || screen != currentScreen)
                {
                    currentConsole = console;
                    currentScreen  = screen;

                    if (currentConsole != null && currentScreen != null)
                        currentConsole.AttachScreen(currentScreen);
                }
            }
            else if (currentConsole != null)
            {
                currentConsole.DetachScreen();
                currentConsole.PowerOff();
                currentConsole = null;
                currentScreen = null;
            }
        }
    }
}
