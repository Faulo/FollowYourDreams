using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class MoveToGround : MonoBehaviour {
        [SerializeField]
        Transform context;

        Vector3 position;

        void FixedUpdate() {
            if (position != context.position) {
                position = context.position;
                transform.position = GridManager.instance.RoundToTileCenterGround(position, gameObject.layer);
            }
        }
    }
}
