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
            : Vector2.SignedAngle(intendedMove, Vector2.up);
        float intendedSpeed => intendedMove.magnitude;
        [SerializeField]
        Direction intendedDirection;
        float torque;
        float acceleration;
        [SerializeField]
        bool intendsToRun;
        float maxSpeed => intendsToRun
            ? settings.runSpeed
            : settings.walkSpeed;
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

            currentSpeed = Mathf.SmoothDampAngle(currentSpeed, intendedSpeed * maxSpeed, ref acceleration, settings.speedSmoothing);

            intendedDirection.Set(intendedRotation);
            currentDirection = intendedDirection switch {
                Direction.Up => AvatarDirection.Up,
                Direction.UpRight => AvatarDirection.UpLeft,
                Direction.Right => AvatarDirection.Left,
                Direction.DownRight => AvatarDirection.DownLeft,
                Direction.Down => AvatarDirection.Down,
                Direction.DownLeft => AvatarDirection.DownLeft,
                Direction.Left => AvatarDirection.Left,
                Direction.UpLeft => AvatarDirection.UpLeft,
                _ => throw new System.NotImplementedException(intendedDirection.ToString()),
            };

            switch (intendedDirection) {
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

            currentAnimation = intendedSpeed > 0.1f
                ? intendsToRun
                    ? AvatarAnimation.Run
                    : AvatarAnimation.Walk
                : AvatarAnimation.Idle;
        }

        void ProcessCharacter() {
            var motion = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward * currentSpeed * Time.deltaTime;
            attachedCharacter.Move(motion);
        }

        public void OnMove(InputValue value) {
            intendedMove = value.Get<Vector2>();
        }

        public void OnRun(InputValue value) {
            intendsToRun = value.isPressed;
        }
    }
}
