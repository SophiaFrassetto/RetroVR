using UnityEngine;

namespace retrovr.system.physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class XRPhysicalObject : MonoBehaviour
    {
        protected Rigidbody rb;

        [Header("Physics Settings")]
        public bool startKinematic = false;
        public float sleepVelocityThreshold = 0.01f;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.isKinematic = startKinematic;
            rb.useGravity = !startKinematic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        protected virtual void FixedUpdate()
        {
            if (!rb.isKinematic && rb.linearVelocity.sqrMagnitude < sleepVelocityThreshold)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }

        public virtual void OnGrabbed()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        public virtual void OnReleased()
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        public virtual void ForceKinematic()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
