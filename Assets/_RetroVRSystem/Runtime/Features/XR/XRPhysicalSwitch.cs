using UnityEngine;
using UnityEngine.Events;

namespace retrovr.system.interaction
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class XRPhysicalSwitch : MonoBehaviour
    {
        public enum SwitchAxis { X, Y, Z }

        [Header("Switch Settings")]
        public SwitchAxis axis = SwitchAxis.Y;
        public float travel = 0.02f;
        public float snapSpeed = 10f;

        [Header("Events")]
        public UnityEvent onStateA;
        public UnityEvent onStateB;

        private Vector3 startLocalPos;
        private bool stateA = true;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            ConfigurePhysics();
        }

        void Start()
        {
            startLocalPos = transform.localPosition;
        }

        void Update()
        {
            float delta = GetDelta();

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

            Vector3 target = stateA ? startLocalPos : startLocalPos + GetAxisVector() * travel;
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * snapSpeed);
        }

        float GetDelta()
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

        void ConfigurePhysics()
        {
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.mass = 0.1f;
            rb.linearDamping = 10f;
            rb.angularDamping = 10f;

            rb.constraints =
                RigidbodyConstraints.FreezeRotation |
                (axis != SwitchAxis.X ? RigidbodyConstraints.FreezePositionX : 0) |
                (axis != SwitchAxis.Y ? RigidbodyConstraints.FreezePositionY : 0) |
                (axis != SwitchAxis.Z ? RigidbodyConstraints.FreezePositionZ : 0);
        }
    }
}
