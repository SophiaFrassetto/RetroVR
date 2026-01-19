using UnityEngine;
using System.Collections.Generic;

namespace retrovr.system.cable
{
    public class CableRuntime : MonoBehaviour
    {
        public GameObject segmentPrefab;
        public int segmentCount = 14;
        public float segmentLength = 0.025f;

        private readonly List<Rigidbody> segments = new();
        private CableEndFollower follower;

        public void Spawn(Transform socketOrigin, Transform followTarget)
        {
            Rigidbody previous = null;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 pos = socketOrigin.position - socketOrigin.forward * (segmentLength * i);
                var seg = Instantiate(segmentPrefab, pos, socketOrigin.rotation, transform);

                var rb = seg.GetComponent<Rigidbody>();
                segments.Add(rb);

                if (previous != null)
                {
                    var joint = seg.AddComponent<ConfigurableJoint>();
                    joint.connectedBody = previous;

                    joint.xMotion = ConfigurableJointMotion.Limited;
                    joint.yMotion = ConfigurableJointMotion.Limited;
                    joint.zMotion = ConfigurableJointMotion.Limited;

                    joint.linearLimit = new SoftJointLimit { limit = segmentLength };

                    joint.angularXMotion = ConfigurableJointMotion.Limited;
                    joint.angularYMotion = ConfigurableJointMotion.Limited;
                    joint.angularZMotion = ConfigurableJointMotion.Limited;

                    joint.lowAngularXLimit  = new SoftJointLimit { limit = -20f };
                    joint.highAngularXLimit = new SoftJointLimit { limit = 20f };
                    joint.angularYLimit     = new SoftJointLimit { limit = 20f };
                    joint.angularZLimit     = new SoftJointLimit { limit = 20f };
                }

                previous = rb;
            }

            // Primeiro segmento fixo no socket
            segments[0].isKinematic = true;
            segments[0].transform.position = socketOrigin.position;
            segments[0].transform.rotation = socketOrigin.rotation;

            // Último segmento segue a mão
            follower = segments[^1].gameObject.AddComponent<CableEndFollower>();
            follower.Attach(followTarget);
        }

        public void LockToSocket(Transform socket)
        {
            if (follower != null)
                Destroy(follower);

            var last = segments[^1];
            last.isKinematic = true;
            last.transform.position = socket.position;
            last.transform.rotation = socket.rotation;
        }

        public void DestroyCable()
        {
            Destroy(gameObject);
        }
    }
}
