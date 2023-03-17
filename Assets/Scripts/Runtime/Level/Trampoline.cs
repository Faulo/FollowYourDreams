using FollowYourDreams.Avatar;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class Trampoline : MonoBehaviour, ICollidable {
        [SerializeField, Range(0, 10)]
        float minBounceSpeed = 1;
        [SerializeField, Range(0, 100)]
        float bounceSpeed = 10;

        public void OnCollide(ControllerColliderHit hit, AvatarController avatar) {
            if (avatar.currentVerticalSpeed < -minBounceSpeed) {
                avatar.currentVerticalSpeed = bounceSpeed;
            }
        }
    }
}
