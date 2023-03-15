using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowYourDreams.Avatar {
    sealed class AvatarController : MonoBehaviour {
        public event Action<AvatarAnimation> onAnimationChange;


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
        [SerializeField, ReadOnly]
        float currentRotation = 0;
        [SerializeField, ReadOnly]
        float currentHorizontalSpeed = 0;
        [SerializeField, ReadOnly]
        float currentVerticalSpeed = 0;
        [SerializeField, ReadOnly]
        AvatarDirection currentDirection = AvatarDirection.Down;
        AvatarAnimation currentAnimation {
            get => m_currentAnimation;
            set {
                if (m_currentAnimation != value) {
                    m_currentAnimation = value;
                    onAnimationChange?.Invoke(value);
                }
            }
        }
        [SerializeField, ReadOnly]
        AvatarAnimation m_currentAnimation = AvatarAnimation.Idle;

        [Header("Input")]
        [SerializeField, ReadOnly]
        Vector2 intendedMove;
        float intendedRotation => intendedMove == Vector2.zero
            ? currentRotation
            : Vector2.SignedAngle(intendedMove, Vector2.up);
        float intendedSpeed => intendedMove.magnitude;
        [SerializeField, ReadOnly]
        Direction intendedDirection;
        float torque;
        float acceleration;
        [SerializeField, ReadOnly]
        bool intendsToRun;
        float maxSpeed => intendsToRun
            ? settings.runSpeed
            : settings.walkSpeed;
        [SerializeField, ReadOnly]
        bool intendsToJump;
        [SerializeField, ReadOnly]
        bool isJumping;

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
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("animation", currentAnimation.ToString());
        }

        void ProcessInput() {
            currentRotation = Mathf.SmoothDampAngle(currentRotation, intendedRotation, ref torque, settings.rotationSmoothing);

            currentHorizontalSpeed = Mathf.SmoothDampAngle(currentHorizontalSpeed, intendedSpeed * maxSpeed, ref acceleration, settings.speedSmoothing);

            if (intendsToJump) {
                intendsToJump = false;
                if (attachedCharacter.isGrounded) {
                    currentVerticalSpeed += settings.jumpSpeed;
                    isJumping = true;
                }
            }
            if (isJumping) {
                if (currentVerticalSpeed <= 0) {
                    isJumping = false;
                }
            }

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

            currentAnimation = CalculateAnimation();
        }

        AvatarAnimation CalculateAnimation() {
            if (isJumping) {
                return AvatarAnimation.Jump;
            }
            if (!attachedCharacter.isGrounded) {
                return AvatarAnimation.Fall;
            }
            return intendedSpeed > 0.1f
                ? intendsToRun
                    ? AvatarAnimation.Run
                    : AvatarAnimation.Walk
                : AvatarAnimation.Idle;
        }

        void ProcessCharacter() {
            currentVerticalSpeed += Physics.gravity.y * Time.deltaTime;
            var motion = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward * currentHorizontalSpeed * Time.deltaTime;
            motion.y += currentVerticalSpeed * Time.deltaTime;
            attachedCharacter.Move(motion);
            if (attachedCharacter.isGrounded) {
                currentVerticalSpeed = 0;
            }
        }

        public void OnMove(InputValue value) {
            intendedMove = value.Get<Vector2>();
        }

        public void OnRun(InputValue value) {
            intendsToRun = value.isPressed;
        }

        public void OnJump(InputValue value) {
            intendsToJump = value.isPressed;
        }
    }
}
