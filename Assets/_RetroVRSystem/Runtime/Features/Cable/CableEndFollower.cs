using UnityEngine;

namespace retrovr.system.cable
{
    public class CableEndFollower : MonoBehaviour
    {
        private Transform target;
        private Rigidbody rb;

        public void Attach(Transform followTarget)
        {
            target = followTarget;
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        void FixedUpdate()
        {
            if (target != null)
                rb.MovePosition(target.position);
        }
    }
}
