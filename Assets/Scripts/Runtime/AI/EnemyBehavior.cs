using System;
using FollowYourDreams.Avatar;
using MyBox;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.AI;

namespace FollowYourDreams.AI {
    sealed class EnemyBehavior : MonoBehaviour {
        [Header("Setup")]
        [SerializeField]
        GameManager manager = default;

        [Space]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        SpriteRenderer attachedRenderer;
        [SerializeField]
        NavMeshAgent attachedAgent = default;

        [Header("Config")]
        [SerializeField, Range(0f, 50f)]
        float lineOfSightDistance = 10.0f;
        [SerializeField, Range(0f, 5f)]
        float snackRange = 1f;

        AvatarController target = default;
        bool targetAquired = false;
        bool snacked = false;

        void OnValidate() {
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
            if (!attachedAgent) {
                TryGetComponent(out attachedAgent);
            }
        }

        void FixedUpdate() {
            if (manager.currentDimension != Dimension.NightmareRealm) {
                ResetEnemy();
                return;
            }

            if (!target) {
                target = FindAnyObjectByType<AvatarController>();
            }
            if (!target || target.isInteracting) {
                return;
            }
            if (!targetAquired) {
                targetAquired = Vector3.Distance(transform.position, target.transform.position) < lineOfSightDistance;
            }
            if (targetAquired && !snacked) {
                attachedAgent.SetDestination(target.transform.position);
                if (Vector3.Distance(transform.position, target.transform.position) < snackRange) {
                    Snack();
                }
            }
        }
        void Update() {
            if (manager.currentDimension != Dimension.NightmareRealm) {
                return;
            }

            currentAnimation = (CalculateDirection(), CalculateAnimation());
        }

        [SerializeField, ReadOnly]
        Direction currentDirection = Direction.Down;

        (Direction direction, AvatarAnimation animation) currentAnimation {
            set {
                if (m_currentAnimation == value) {
                    return;
                }
                m_currentAnimation = value;
                var direction = value.direction switch {
                    Direction.Up => AvatarDirection.Up,
                    Direction.UpRight => AvatarDirection.UpLeft,
                    Direction.Right => AvatarDirection.Left,
                    Direction.DownRight => AvatarDirection.DownLeft,
                    Direction.Down => AvatarDirection.Down,
                    Direction.DownLeft => AvatarDirection.DownLeft,
                    Direction.Left => AvatarDirection.Left,
                    Direction.UpLeft => AvatarDirection.UpLeft,
                    _ => throw new NotImplementedException(value.direction.ToString()),
                };
                attachedAnimator.Play(AvatarSettings.GetAnimationName(direction, value.animation));
                switch (value.direction) {
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
        }
        (Direction direction, AvatarAnimation animation) m_currentAnimation;

        Direction CalculateDirection() {
            var movement = attachedAgent.desiredVelocity.SwizzleXZ();
            if (movement.magnitude > 0.1f) {
                float angle = Vector2.SignedAngle(movement, Vector2.up);
                currentDirection.Set(angle);
            }
            return currentDirection;
        }

        AvatarAnimation CalculateAnimation() {
            return targetAquired
                ? AvatarAnimation.Walk
                : AvatarAnimation.Idle;
        }

        void Snack() {
            snacked = true;
            target.Die();
        }

        void ResetEnemy() {
            attachedAgent.SetDestination(transform.parent.position);
            targetAquired = false;
            snacked = false;
        }
    }
}
