using System;
using System.Collections;
using MyBox;
using Slothsoft.UnityExtensions;
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
        AvatarMovement movement => settings.movement;

        [Header("Runtime")]
        [SerializeField, ReadOnly]
        float currentRotation = 0;
        [SerializeField, ReadOnly]
        float currentHorizontalSpeed = 0;
        [SerializeField, ReadOnly]
        float currentVerticalSpeed = 0;
        [SerializeField, ReadOnly]
        AvatarDirection currentDirection = AvatarDirection.Down;
        public AvatarAnimation currentAnimation {
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
        float maxSpeed => isGliding
            ? movement.glideSpeed
            : intendsToRun
                ? movement.runSpeed
                : movement.walkSpeed;
        float speedSmoothing => isGliding
            ? movement.glideSmoothing
            : intendedSpeed > 0.1f
                ? movement.accelerationSmoothing
                : movement.decelerationSmoothing;
        [SerializeField, ReadOnly]
        bool intendsToJump;
        [SerializeField, ReadOnly]
        bool intendsToJumpStart;
        [SerializeField, ReadOnly]
        bool isJumping;
        [SerializeField, ReadOnly]
        bool isGliding;

        // readonly HashSet<IInteractable> interactablePool = new();
        IInteractable currentInteractable;
        Coroutine interactableRoutine;
        bool isInteracting => interactableRoutine != null;

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
            if (currentAnimation == AvatarAnimation.None) {
                attachedRenderer.enabled = false;
            } else {
                attachedRenderer.enabled = true;
                attachedAnimator.Play(AvatarSettings.GetAnimationName(currentDirection, currentAnimation));
                attachedAnimator.Update(Time.deltaTime);
            }
        }

        void ProcessInput() {
            if (isInteracting) {
                return;
            }

            currentRotation = Mathf.SmoothDampAngle(currentRotation, intendedRotation, ref torque, movement.rotationSmoothing);

            float intendedSpeed = isGliding
                ? 1
                : this.intendedSpeed;

            currentHorizontalSpeed = Mathf.SmoothDampAngle(currentHorizontalSpeed, intendedSpeed * maxSpeed, ref acceleration, speedSmoothing);

            if (isGliding) {
                if (!intendsToJump && movement.canCancelGlide) {
                    isGliding = false;
                }
                if (attachedCharacter.isGrounded) {
                    isGliding = false;
                    PlayAnimation(movement.glideToLandAnimation, movement.glideToLandDuration);
                    return;
                }
            } else if (isJumping) {
                if (!intendsToJump || currentVerticalSpeed <= 0) {
                    isJumping = false;
                    currentVerticalSpeed *= movement.jumpStopMultiplier;
                }
            } else if (intendsToJumpStart) {
                if (attachedCharacter.isGrounded) {
                    intendsToJumpStart = false;
                    if (!movement.canJump) {
                        PlayAnimation(movement.glideToLandAnimation, movement.glideToLandDuration);
                        return;
                    }
                    currentVerticalSpeed += movement.jumpSpeed;
                    isJumping = true;
                } else {
                    intendsToJumpStart = false;
                    currentVerticalSpeed = movement.glideVerticalBoost;
                    currentHorizontalSpeed += movement.glideHorizontalBoost;
                    isGliding = true;
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
                _ => throw new NotImplementedException(intendedDirection.ToString()),
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
            if (isGliding) {
                return AvatarAnimation.Glide;
            }
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
            if (isInteracting) {
                return;
            }
            float gravity = Physics.gravity.y * Time.deltaTime;
            if (isJumping) {
                gravity *= movement.jumpGravityMultiplier;
            }
            if (isGliding) {
                gravity *= movement.glideGravityMultiplier;
            }
            currentVerticalSpeed += gravity;
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
            intendsToJumpStart = value.isPressed;
        }

        public void OnInteract(InputValue value) {
            if (value.isPressed) {
                if (!isInteracting && currentInteractable != null) {
                    InteractWith(currentInteractable);
                }
            }
        }

        void InteractWith(IInteractable interactable) {
            interactableRoutine = StartCoroutine(Interact_Co(interactable.Interact_Co(this)));
        }
        void PlayAnimation(AvatarAnimation animation, float duration) {
            interactableRoutine = StartCoroutine(Interact_Co(PlayAnimation_Co(animation, duration)));
        }

        IEnumerator PlayAnimation_Co(AvatarAnimation animation, float duration) {
            currentAnimation = animation;
            yield return Wait.forSeconds[duration];
        }

        IEnumerator Interact_Co(IEnumerator interaction) {
            yield return interaction;
            interactableRoutine = null;
        }

        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<IInteractable>(out var newInteractable)) {
                if (currentInteractable != null) {
                    currentInteractable.Deselect();
                }
                currentInteractable = newInteractable;
                newInteractable.Select();
            }
        }
        void OnTriggerExit(Collider other) {
            if (other.TryGetComponent<IInteractable>(out var oldInteractable)) {
                if (currentInteractable == oldInteractable) {
                    currentInteractable.Deselect();
                }
                currentInteractable = null;
            }
        }
    }
}
