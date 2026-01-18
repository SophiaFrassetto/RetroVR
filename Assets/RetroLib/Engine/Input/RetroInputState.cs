namespace RetroLib.Engine.Input
{
    public class RetroInputState
    {
        // [port][button]
        private readonly bool[,] buttons = new bool[4, 32];

        public void SetButton(int port, int button, bool pressed)
        {
            buttons[port, button] = pressed;
        }

        public bool GetButton(int port, int button)
        {
            return buttons[port, button];
        }

        public void Clear()
        {
            for (int p = 0; p < 4; p++)
                for (int b = 0; b < 32; b++)
                    buttons[p, b] = false;
        }
    }
}
