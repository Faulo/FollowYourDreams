using FMODUnity;
using FollowYourDreams.Avatar;
using UnityEngine;
using UnityEngine.Events;

namespace FollowYourDreams.Level {
    sealed class AutoPickUp : MonoBehaviour, ICollidable {
        [SerializeField]
        EventReference onCollectAudio = new();
        [SerializeField]
        UnityEvent<Transform> onCollect = new();

        public void OnCollide(ControllerColliderHit hit, AvatarController avatar) {
            onCollectAudio.PlayOnce();
            onCollect.Invoke(transform);
            gameObject.SetActive(false);
        }
    }
}
