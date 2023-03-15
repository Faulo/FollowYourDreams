using System;
using System.Collections;
using FMODUnity;
using Slothsoft.UnityExtensions;
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

        [Header("Go To Sleep")]
        [SerializeField]
        EventReference goToSleepEvent = new();

        [Header("Wake Up")]
        [SerializeField]
        EventReference wakeUpEvent = new();

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
                case AvatarAnimation.Jump:
                    PlayOnce(jumpEvent);
                    break;
                case AvatarAnimation.Land:
                    PlayOnce(landEvent);
                    break;
                case AvatarAnimation.Climb:
                    PlayOnce(climbEvent);
                    break;
                case AvatarAnimation.Interact:
                    PlayOnce(interactEvent);
                    break;
                case AvatarAnimation.Walk:
                    PlayRepeatedly(stepEvent, walkInterval);
                    break;
                case AvatarAnimation.Run:
                    PlayRepeatedly(stepEvent, runInterval);
                    break;
            }
        }

        void PlayOnce(in EventReference reference) {
            var instance = RuntimeManager.CreateInstance(reference);
            instance.start();
        }
        void PlayRepeatedly(in EventReference reference, float interval) {
            coroutine = StartCoroutine(PlayRepeatedly_Co(reference, interval));
        }

        IEnumerator PlayRepeatedly_Co(EventReference reference, float interval) {
            while (true) {
                var instance = RuntimeManager.CreateInstance(reference);
                instance.start();
                yield return Wait.forSeconds[interval];
            }
        }
    }
}
