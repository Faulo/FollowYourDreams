using System;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class AvatarMovement : ScriptableAsset {
        [SerializeField, Range(0, 10)]
        public float rotationSmoothing = 0.1f;
        [SerializeField, Range(0, 10)]
        public float accelerationSmoothing = 0.1f;
        [SerializeField, Range(0, 10)]
        public float decelerationSmoothing = 0.1f;
        [SerializeField, Range(0, 10)]
        public float walkSpeed = 1;
        [SerializeField, Range(0, 10)]
        public float runSpeed = 2;

        [Header("Jump")]
        [SerializeField]
        public bool canJump = true;
        [SerializeField, Range(0, 10)]
        public float jumpSpeed = 5;
        [SerializeField, Range(0, 10)]
        public float jumpStopMultiplier = 0.25f;
        [SerializeField, Range(0, 10)]
        public float jumpGravityMultiplier = 0.25f;

        [Header("Glide")]
        [SerializeField, Range(0, 10)]
        public float glideSpeed = 2;
        [SerializeField, Range(0, 10)]
        public float glideVerticalBoost = 1;
        [SerializeField, Range(0, 10)]
        public float glideHorizontalBoost = 1;
        [SerializeField, Range(0, 10)]
        public float glideGravityMultiplier = 0.1f;
        [SerializeField, Range(0, 10)]
        public float glideSmoothing = 0.1f;
        [SerializeField]
        public bool canCancelGlide = true;
        [SerializeField]
        public AvatarAnimation glideToLandAnimation = AvatarAnimation.Land;
        [SerializeField, Range(0, 10)]
        public float glideToLandDuration = 0.1f;

        [Header("HighJump")]
        [SerializeField, Range(0, 10)]
        public float highJumpChargeMinimum = 0;
        [SerializeField, Range(0, 10)]
        public float highJumpChargeMaximum = 1f;
        [SerializeField, Range(0, 20)]
        public float highJumpSpeed = 5f;
        [SerializeField, Range(0, 10)]
        public float highJumpGravityMultiplier = 0.25f;
    }
}
