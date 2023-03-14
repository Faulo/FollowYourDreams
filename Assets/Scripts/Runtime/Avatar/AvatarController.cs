using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class AvatarController : MonoBehaviour {
        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        SpriteRenderer attachedRenderer;
        [SerializeField]
        CharacterController attachedCharacter;
        [SerializeField]
        AvatarSettings settings;

        [Header("Runtime")]
        [SerializeField]
        float currentRotation = 0;
        [SerializeField]
        float currentSpeed = 0;
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
            if (!attachedCharacter) {
                TryGetComponent(out attachedCharacter);
            }
        }

        void FixedUpdate() {
            var motion = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward * currentSpeed;
            attachedCharacter.Move(motion);
        }

        void Update() {
            attachedAnimator.Play(AvatarSettings.GetAnimationName(currentDirection, currentAnimation));
        }
    }
}
