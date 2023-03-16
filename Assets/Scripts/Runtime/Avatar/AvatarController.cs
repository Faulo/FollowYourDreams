using System;
using System.Collections;
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
        float maxSpeed => intendsToRun
            ? settings.runSpeed
            : settings.walkSpeed;
        float speedSmoothing => isGliding
            ? settings.glideSmoothing
            : settings.speedSmoothing;
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
        bool isInteracting;

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

            currentRotation = Mathf.SmoothDampAngle(currentRotation, intendedRotation, ref torque, settings.rotationSmoothing);

            float intendedSpeed = isGliding
                ? 1
                : this.intendedSpeed;

            currentHorizontalSpeed = Mathf.SmoothDampAngle(currentHorizontalSpeed, intendedSpeed * maxSpeed, ref acceleration, speedSmoothing);

            if (isGliding) {
                if (!intendsToJump) {
                    isGliding = false;
                }
            } else if (isJumping) {
                if (!intendsToJump || currentVerticalSpeed <= 0) {
                    isJumping = false;
                    currentVerticalSpeed *= settings.jumpStopMultiplier;
                }
            } else if (intendsToJumpStart) {
                if (attachedCharacter.isGrounded) {
                    intendsToJumpStart = false;
                    currentVerticalSpeed += settings.jumpSpeed;
                    isJumping = true;
                } else {
                    intendsToJumpStart = false;
                    currentVerticalSpeed = settings.glideVerticalBoost;
                    currentHorizontalSpeed += settings.glideHorizontalBoost;
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
                gravity *= settings.jumpGravityMultiplier;
            }
            if (isGliding) {
                gravity *= settings.glideGravityMultiplier;
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
                    interactableRoutine = StartCoroutine(Interact_Co(currentInteractable));
                }
            }
        }

        IEnumerator Interact_Co(IInteractable interactable) {
            isInteracting = true;
            yield return interactable.Interact_Co(this);
            isInteracting = false;
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
