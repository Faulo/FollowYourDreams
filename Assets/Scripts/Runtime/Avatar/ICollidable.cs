using UnityEngine;

namespace FollowYourDreams.Avatar {
    interface ICollidable {
        void OnCollide(ControllerColliderHit hit, AvatarController avatar);
    }
}
