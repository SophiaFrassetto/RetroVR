using UnityEngine;

namespace RetroLib.Engine.Input
{
    public class KeyboardInputProvider : IRetroInputProvider
    {
        public RetroInputState State { get; } = new RetroInputState();

        public void Poll()
        {
            State.SetButton(0, RetroButtons.A, UnityEngine.Input.GetKey(KeyCode.X));
            State.SetButton(0, RetroButtons.B, UnityEngine.Input.GetKey(KeyCode.Z));
            State.SetButton(0, RetroButtons.START, UnityEngine.Input.GetKey(KeyCode.Return));
            State.SetButton(0, RetroButtons.SELECT, UnityEngine.Input.GetKey(KeyCode.RightShift));
            State.SetButton(0, RetroButtons.UP, UnityEngine.Input.GetKey(KeyCode.UpArrow));
            State.SetButton(0, RetroButtons.DOWN, UnityEngine.Input.GetKey(KeyCode.DownArrow));
            State.SetButton(0, RetroButtons.LEFT, UnityEngine.Input.GetKey(KeyCode.LeftArrow));
            State.SetButton(0, RetroButtons.RIGHT, UnityEngine.Input.GetKey(KeyCode.RightArrow));
        }
    }
}
