using UnityEngine;
using UnityEngine.Events;

namespace retrovr.system.interaction
{
    public class XRPhysicalSwitch : MonoBehaviour
    {
        public enum SwitchAxis { X, Y, Z }

        public SwitchAxis axis = SwitchAxis.Z;
        public float travel = 0.014f;
        public float snapSpeed = 15f;

        public UnityEvent onStateA;
        public UnityEvent onStateB;

        private Vector3 startLocalPos;
        private bool stateA = true;

        void Start()
        {
            startLocalPos = transform.localPosition;
        }

        void Update()
        {
            float delta = GetLocalDelta();

            if (stateA && delta > travel * 0.5f)
            {
                stateA = false;
                onStateB?.Invoke();
            }
            else if (!stateA && delta < -travel * 0.5f)
            {
                stateA = true;
                onStateA?.Invoke();
            }

            Vector3 targetLocal = stateA
                ? startLocalPos
                : startLocalPos + GetAxisVector() * travel;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetLocal,
                Time.deltaTime * snapSpeed
            );
        }

        float GetLocalDelta()
        {
            Vector3 local = transform.localPosition - startLocalPos;
            return axis switch
            {
                SwitchAxis.X => local.x,
                SwitchAxis.Y => local.y,
                _ => local.z
            };
        }

        Vector3 GetAxisVector()
        {
            return axis switch
            {
                SwitchAxis.X => Vector3.right,
                SwitchAxis.Y => Vector3.up,
                _ => Vector3.forward
            };
        }
    }
}
