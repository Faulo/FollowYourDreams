using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class AvatarController : MonoBehaviour {
        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        SpriteRenderer attachedRenderer;
        [SerializeField]
        AvatarSettings settings;

        [Header("Runtime")]
        [SerializeField]
        AvatarDirection currentDirection = AvatarDirection.Down;
        [SerializeField]
        AvatarAnimation currentAnimation = AvatarAnimation.idle;

        void OnValidate() {
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
        }

        void Update() {
            attachedAnimator.Play(AvatarSettings.GetAnimationName(currentDirection, currentAnimation));
        }
    }
}
