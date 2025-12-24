using UnityEngine;
using UnityEngine.Events;

namespace retrovr.system.interaction
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class XRPushButton : MonoBehaviour
    {
        [Header("Press Settings")]
        public float pressDepth = 0.01f;
        public float returnSpeed = 10f;

        [Header("Auto Physics Setup")]
        public bool autoConfigurePhysics = true;

        [Header("Events")]
        public UnityEvent onPressed;
        public UnityEvent onReleased;

        private Vector3 startLocalPos;
        private bool pressed;

        private Rigidbody rb;
        private Collider col;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            if (autoConfigurePhysics)
                ConfigurePhysics();
        }

        void Start()
        {
            startLocalPos = transform.localPosition;
        }

        void Update()
        {
            float delta = startLocalPos.y - transform.localPosition.y;

            if (!pressed && delta > pressDepth)
            {
                pressed = true;
                onPressed?.Invoke();
            }

            if (pressed && delta < pressDepth * 0.5f)
            {
                pressed = false;
                onReleased?.Invoke();
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, startLocalPos, Time.deltaTime * returnSpeed);
        }

        void ConfigurePhysics()
        {
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.mass = 0.1f;
            rb.linearDamping = 10f;
            rb.angularDamping = 10f;

            rb.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;

            col.isTrigger = false;
        }
    }
}
