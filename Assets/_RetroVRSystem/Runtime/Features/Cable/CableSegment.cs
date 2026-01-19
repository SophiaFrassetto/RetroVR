using UnityEngine;

namespace retrovr.system.cable
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class CableSegment : MonoBehaviour
    {
        void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            rb.mass = 0.04f;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.5f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}
