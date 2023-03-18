using System;
using FMODUnity;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class AvatarAudio : ComponentFeature<AvatarController> {
        [Header("Steps")]
        [SerializeField]
        EventReference stepEvent = new();
        [SerializeField, Range(0, 10)]
        float walkInterval = 0.2f;
        [SerializeField, Range(0, 10)]
        float runInterval = 0.1f;

        [Header("Interact")]
        [SerializeField]
        EventReference interactEvent = new();

        [Header("Jump")]
        [SerializeField]
        EventReference jumpEvent = new();

        [Header("Land")]
        [SerializeField]
        EventReference landEvent = new();

        [Header("Climb")]
        [SerializeField]
        EventReference climbEvent = new();

        [Header("Stumble")]
        [SerializeField]
        EventReference stumbleEvent = new();

        [Header("Glide")]
        [SerializeField]
        EventReference glideEvent = new();

        Coroutine coroutine;

        void OnEnable() {
            observedComponent.onAnimationChange += OnAnimationChange;
        }
        void OnDisable() {
            observedComponent.onAnimationChange -= OnAnimationChange;
        }

        void OnAnimationChange(AvatarAnimation animation) {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            switch (animation) {
                case AvatarAnimation.Glide:
                    glideEvent.PlayOnce();
                    break;
                case AvatarAnimation.Jump:
                    jumpEvent.PlayOnce();
                    break;
                case AvatarAnimation.Land:
                    landEvent.PlayOnce();
                    break;
                case AvatarAnimation.Climb:
                    climbEvent.PlayOnce();
                    break;
                case AvatarAnimation.Stumble:
                    stumbleEvent.PlayOnce();
                    break;
                case AvatarAnimation.Interact:
                    interactEvent.PlayOnce();
                    break;
                case AvatarAnimation.Walk:
                    coroutine = stepEvent.PlayRepeatedly(this, walkInterval);
                    break;
                case AvatarAnimation.Push:
                    coroutine = stepEvent.PlayRepeatedly(this, walkInterval);
                    break;
                case AvatarAnimation.Run:
                    coroutine = stepEvent.PlayRepeatedly(this, runInterval);
                    break;
            }
        }
    }
}
