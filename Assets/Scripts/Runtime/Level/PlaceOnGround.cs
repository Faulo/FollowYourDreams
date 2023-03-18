using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class PlaceOnGround : MonoBehaviour {
        [SerializeField]
        Transform context;
        [SerializeField]
        LayerMask hitLayers;
        [SerializeField]
        Vector3 castOffset = Vector3.up;
        [SerializeField]
        Vector3 groundOffset = Vector3.zero;

        RaycastHit[] hits = new RaycastHit[32];
        int hitCount;
        void FixedUpdate() {
            hitCount = Physics.RaycastNonAlloc(context.position + castOffset, Vector3.down, hits, 100, hitLayers | (1 << context.gameObject.layer));
            var point = Vector3.zero;
            bool wasHit = false;
            for (int i = 0; i < hitCount; i++) {
                var hit = hits[i];
                if (hit.transform != context) {
                    if (!wasHit || point.y < hit.point.y) {
                        wasHit = true;
                        point = hit.point;
                    }
                }
            }
            if (wasHit) {
                transform.position = point + groundOffset;
            }
        }
    }
}
