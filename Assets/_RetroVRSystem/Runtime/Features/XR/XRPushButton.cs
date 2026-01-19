using UnityEngine;
using UnityEngine.Events;

namespace retrovr.system.interaction
{
    public class XRPushButton : MonoBehaviour
    {
        [Header("Button Movement")]
        public float pressDepth = 0.01f;
        public float returnSpeed = 18f;

        [Header("Events")]
        public UnityEvent onPressed;
        public UnityEvent onReleased;

        private Vector3 startLocalPos;
        private bool pressed;

        void Start()
        {
            startLocalPos = transform.localPosition;
        }

        void Update()
        {
            float delta = startLocalPos.y - transform.localPosition.y;

            if (!pressed && delta >= pressDepth)
            {
                pressed = true;
                onPressed?.Invoke();
            }
            else if (pressed && delta <= pressDepth * 0.3f)
            {
                pressed = false;
                onReleased?.Invoke();
            }

            // retorno suave
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startLocalPos,
                Time.deltaTime * returnSpeed
            );
        }
    }
}
