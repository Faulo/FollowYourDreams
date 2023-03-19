using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
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
        [SerializeField]
        BedState lastUsedBed;
        readonly Stack<BedState> allUsedBeds = new();
        public void AddBed(BedState bed) => allUsedBeds.Push(bed);
        BedState PopBed() {
            return allUsedBeds.Count > 0
                ? allUsedBeds.Pop()
                : lastUsedBed;
        }
        [Header("Runtime")]
        [SerializeField, ReadOnly]
        float currentRotation = 0;
        public Vector3 currentForward => Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
        [SerializeField, ReadOnly]
        float currentHorizontalSpeed = 0;
        [SerializeField, ReadOnly]
        public float currentVerticalSpeed = 0;
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
        bool intendsToMove => intendedSpeed > 0.1f;
        [SerializeField, ReadOnly]
        bool intendsToRun;
        float maxSpeed => isGliding
            ? movement.glideSpeed
            : intendsToHighJump
                ? 0
                : intendsToRun
                    ? movement.runSpeed
                    : movement.walkSpeed;
        float speedSmoothing => isGliding
            ? movement.glideSmoothing
            : intendsToMove
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

        [Space]
        [SerializeField, ReadOnly]
        bool intendsToHighJump;
        [SerializeField, ReadOnly]
        bool intendsToHighJumpStart;
        [SerializeField, ReadOnly]
        float highJumpCharge = 0;
        [SerializeField, ReadOnly]
        bool isHighJumping;

        readonly HashSet<IInteractable> interactablePool = new();
        IInteractable currentInteractable => interactablePool.Count == 0
            ? null
            : interactablePool
                .OrderByDescending(i => i.priority)
                .FirstOrDefault();
        Coroutine interactableRoutine;

        [Header("Carrying")]
        [SerializeField]
        float carryDistance = 0.5f;
        Vector3 carryPosition => transform.position + (currentForward * carryDistance);
        Vector3 leavePosition => GridManager.instance.RoundToTileCenter(transform.position);
        public ICarryable carryable {
            get => m_carryable;
            set {
                if (m_carryable != null) {
                    m_carryable.position = leavePosition;
                }
                m_carryable = value;
                isCarrying = value != null;
                UpdateCarry();
            }
        }
        ICarryable m_carryable;
        bool isCarrying;
        void UpdateCarry() {
            if (isCarrying) {
                m_carryable.position = carryPosition;
            }
        }

        public bool isInteracting => interactableRoutine != null;

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
            UpdateCarry();
        }

        void Update() {
            if (currentAnimation == AvatarAnimation.None) {
                attachedRenderer.enabled = false;
            } else {
                attachedRenderer.enabled = true;
                attachedAnimator.Play(AvatarSettings.GetAnimationName(currentDirection, currentAnimation));
                attachedAnimator.Update(Time.deltaTime);
            }
            // Debug.Log(string.Join(" | ", powers.Select(kv => $"{kv.Key}: {kv.Value}")));
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
            } else if (isHighJumping) {
                if (currentVerticalSpeed <= 0) {
                    isHighJumping = false;
                }
            } else if (intendsToJumpStart) {
                if (attachedCharacter.isGrounded) {
                    intendsToJumpStart = false;
                    if (!movement.canJump) {
                        PlayAnimation(movement.glideToLandAnimation, movement.glideToLandDuration);
                        return;
                    }
                    currentVerticalSpeed = movement.jumpSpeed;
                    isJumping = true;
                } else {
                    intendsToJumpStart = false;
                    if (settings.HasPower(Power.Glide) && !isCarrying) {
                        currentVerticalSpeed = movement.glideVerticalBoost;
                        currentHorizontalSpeed += movement.glideHorizontalBoost;
                        isGliding = true;
                    }
                }
            }

            if (attachedCharacter.isGrounded) {
                if (intendsToHighJumpStart && highJumpCharge > movement.highJumpChargeMinimum) {
                    intendsToHighJumpStart = false;
                    float multiplier = Mathf.InverseLerp(0, movement.highJumpChargeMaximum, highJumpCharge);
                    currentVerticalSpeed = movement.highJumpSpeed * multiplier;
                    isHighJumping = true;
                    highJumpCharge = 0;
                } else {
                    if (intendsToHighJump) {
                        highJumpCharge += Time.deltaTime;
                    } else {
                        highJumpCharge = 0;
                    }
                }
            }

            if (!intendsToHighJump) {
                intendedDirection.Set(intendedRotation);
            }

            ProcessIntendedDirection();

            currentAnimation = CalculateAnimation();
        }
        void ProcessIntendedDirection() {
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
        }

        AvatarAnimation CalculateAnimation() {
            if (isCarrying) {
                return intendsToMove
                    ? AvatarAnimation.Push
                    : AvatarAnimation.PushIdle;
            }
            if (isGliding) {
                return AvatarAnimation.Glide;
            }
            if (isJumping) {
                return AvatarAnimation.Jump;
            }
            if (isHighJumping) {
                return AvatarAnimation.HighJump;
            }
            if (!attachedCharacter.isGrounded) {
                return AvatarAnimation.Fall;
            }
            if (intendsToHighJump) {
                return AvatarAnimation.PrepareJump;
            }
            return intendsToMove
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
            if (isHighJumping) {
                gravity *= movement.highJumpGravityMultiplier;
            }
            if (isGliding) {
                gravity *= movement.glideGravityMultiplier;
            }
            currentVerticalSpeed += gravity;
            var motion = currentHorizontalSpeed * Time.deltaTime * currentForward;
            motion.y += currentVerticalSpeed * Time.deltaTime;
            attachedCharacter.Move(motion);
            if (attachedCharacter.isGrounded) {
                m_canClimb = true;
                if (currentVerticalSpeed < 0) {
                    currentVerticalSpeed = 0;
                }
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

        public void OnHighJump(InputValue value) {
            if (!settings.HasPower(Power.HighJump)) {
                return;
            }
            intendsToHighJump = value.isPressed;
            intendsToHighJumpStart = !value.isPressed;
        }

        public void OnInteract(InputValue value) {
            if (value.isPressed) {
                if (!isInteracting && currentInteractable != null) {
                    InteractWith(currentInteractable.Interact_Co);
                }
            }
        }

        void InteractWith(Func<AvatarController, IEnumerator> interact) {
            interactableRoutine = StartCoroutine(Interact_Co(interact(this)));
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
            if (other.TryGetComponent<IInteractable>(out var newInteractable) && newInteractable.isSelectable) {
                var oldInteractable = currentInteractable;
                interactablePool.Add(newInteractable);
                if (oldInteractable != newInteractable) {
                    oldInteractable?.Deselect();
                    currentInteractable.Select();
                }
            }
        }

        void OnTriggerExit(Collider other) {
            if (other.TryGetComponent<IInteractable>(out var newInteractable)) {
                var oldInteractable = currentInteractable;
                interactablePool.Remove(newInteractable);
                if (oldInteractable == newInteractable) {
                    oldInteractable.Deselect();
                    currentInteractable?.Select();
                }
            }
        }

        public void WarpTo(Vector3 position, Direction direction) {
            transform.position = position;
            Physics.SyncTransforms();
            ResetMovement();
            ResetSpeed();
            currentRotation = Vector2.SignedAngle(direction.AsVector2(), Vector2.up);
            intendedDirection = direction;
            intendedMove = Vector2.zero;
            ProcessIntendedDirection();
            attachedCharacter.Move(Vector3.down);
        }

        [SerializeField]
        EventReference deathAudio = new();
        [ContextMenu(nameof(Die))]
        public void Die() {
            ResetMovement();
            ResetSpeed();
            intendedMove = Vector2.zero;
            currentInteractable?.Deselect();
            interactablePool.Clear();
            deathAudio.PlayOnce();
            InteractWith(PopBed().WakeUpIn_Co);
        }
        void ResetMovement() {
            isGliding = false;
            isJumping = false;
            isHighJumping = false;
        }
        void ResetSpeed() {
            currentHorizontalSpeed = 0;
            currentVerticalSpeed = 0;
        }

        void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.gameObject.TryGetComponent<ICollidable>(out var collidable)) {
                collidable.OnCollide(hit, this);
            }

            if (settings.HasPower(Power.Climb) && intendsToRun && !isInteracting && canClimb) {
                if (Mathf.Abs(hit.normal.y) < 0.5f) {
                    var top = hit.collider.ClosestPointOnBounds(hit.point + new Vector3(0, 10, 0));
                    float height = top.y - hit.point.y;
                    if (height <= movement.maxClimbHeight) {
                        m_canClimb = false;
                        ClimbNow();
                    }
                }
            }
        }

        void ClimbNow() {
            Physics.SyncTransforms();
            climbPoint = transform.position + new Vector3(0, movement.climbStep, 0);
            InteractWith(Climb_Co);
        }

        bool canClimb {
            get {
                if (!m_canClimb) {
                    return false;
                }
                var animation = CalculateAnimation();
                return animation == AvatarAnimation.Fall || animation == AvatarAnimation.Glide;
            }
        }
        bool m_canClimb = true;
        Vector3 climbPoint;
        static IEnumerator Climb_Co(AvatarController avatar) {
            avatar.currentAnimation = AvatarAnimation.Climb;
            avatar.ResetMovement();
            avatar.ResetSpeed();
            yield return Wait.forSeconds[avatar.movement.climbDuration];
            avatar.transform.position = avatar.climbPoint;
            Physics.SyncTransforms();
            avatar.currentHorizontalSpeed = avatar.movement.climbHorizontalSpeed;
            avatar.currentVerticalSpeed = avatar.movement.climbVerticalSpeed;
            avatar.isJumping = true;
        }
    }
}
