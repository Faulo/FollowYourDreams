using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class PlaceOnGround : MonoBehaviour {
        [SerializeField]
        Transform context;
        [SerializeField]
        Vector3 castOffset = Vector3.up;
        [SerializeField]
        Vector3 groundOffset = Vector3.zero;
        void FixedUpdate() {
            if (Physics.Raycast(context.position + castOffset, Vector3.down, out var hit, 1 << gameObject.layer)) {
                transform.position = hit.point + groundOffset;
            }
        }
    }
}
