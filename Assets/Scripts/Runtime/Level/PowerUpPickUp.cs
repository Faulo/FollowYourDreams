using FMODUnity;
using FollowYourDreams.Avatar;
using UnityEngine;
using UnityEngine.Events;

namespace FollowYourDreams.Level {
    sealed class PowerUpPickUp : MonoBehaviour, ICollidable {
        [SerializeField]
        Power powerToUnlock;
        [SerializeField]
        EventReference onCollectAudio = new();
        [SerializeField]
        UnityEvent<Transform> onCollect = new();

        public void OnCollide(ControllerColliderHit hit, AvatarController avatar) {
            avatar.GainPower(powerToUnlock);
            onCollectAudio.PlayOnce();
            onCollect.Invoke(transform);
            gameObject.SetActive(false);
        }
    }
}
