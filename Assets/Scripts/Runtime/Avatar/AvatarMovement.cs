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

        [Space]
        [SerializeField, Range(0, 10)]
        public float jumpSpeed = 5;
        [SerializeField, Range(0, 10)]
        public float jumpStopMultiplier = 0.25f;
        [SerializeField, Range(0, 10)]
        public float jumpGravityMultiplier = 0.25f;

        [Space]
        [SerializeField, Range(0, 10)]
        public float glideVerticalBoost = 1;
        [SerializeField, Range(0, 10)]
        public float glideHorizontalBoost = 1;
        [SerializeField, Range(0, 10)]
        public float glideGravityMultiplier = 0.1f;
        [SerializeField, Range(0, 10)]
        public float glideSmoothing = 0.1f;
    }
}
