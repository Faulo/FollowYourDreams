using UnityEngine;
using UnityEngine.InputSystem;

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
        AvatarAnimation currentAnimation = AvatarAnimation.Idle;

        [Header("Input")]
        [SerializeField]
        Vector2 intendedMove;
        float intendedRotation => intendedMove == Vector2.zero
            ? currentRotation
            : Vector2.Angle(Vector2.up, intendedMove);
        float torque;
        [SerializeField]
        bool intendsToJump;

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
            ProcessInput();
            ProcessCharacter();
        }

        void Update() {
            attachedAnimator.Play(AvatarSettings.GetAnimationName(currentDirection, currentAnimation));
            attachedAnimator.Update(Time.deltaTime);
        }

        void ProcessInput() {
            currentRotation = Mathf.SmoothDampAngle(currentRotation, intendedRotation, ref torque, settings.rotationSmoothing);

            var direction = Direction.Down;
            direction.Set(intendedRotation);
            currentDirection = direction switch {
                Direction.Up => AvatarDirection.Up,
                Direction.UpRight => AvatarDirection.UpLeft,
                Direction.Right => AvatarDirection.Left,
                Direction.DownRight => AvatarDirection.DownLeft,
                Direction.Down => AvatarDirection.Down,
                Direction.DownLeft => AvatarDirection.DownLeft,
                Direction.Left => AvatarDirection.Left,
                Direction.UpLeft => AvatarDirection.UpLeft,
                _ => throw new System.NotImplementedException(),
            };

            switch (direction) {
                case Direction.UpLeft:
                case Direction.Left:
                case Direction.DownLeft:
                    attachedRenderer.flipX = false;
                    break;
                case Direction.UpRight:
                case Direction.Right:
                case Direction.DownRight:
                    attachedRenderer.flipX = true;
                    break;
            }
        }

        void ProcessCharacter() {
            var motion = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward * currentSpeed;
            attachedCharacter.Move(motion);
        }

        public void OnMove(InputValue value) {
            intendedMove = value.Get<Vector2>();
        }
    }
}
