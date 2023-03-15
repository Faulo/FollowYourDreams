using System;
using System.Collections;
using FMODUnity;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class AvatarAudio : ComponentFeature<AvatarController> {
        [Header("Steps")]
        [SerializeField]
        EventReference stepEvent;
        [SerializeField, Range(0, 10)]
        float walkInterval = 0.2f;
        [SerializeField, Range(0, 10)]
        float runInterval = 0.1f;

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
                case AvatarAnimation.Walk:
                    coroutine = StartCoroutine(Co_Steps(stepEvent, walkInterval));
                    break;
                case AvatarAnimation.Run:
                    coroutine = StartCoroutine(Co_Steps(stepEvent, runInterval));
                    break;
            }
        }

        IEnumerator Co_Steps(EventReference reference, float interval) {
            while (true) {
                var instance = RuntimeManager.CreateInstance(reference);
                instance.start();
                yield return Wait.forSeconds[interval];
            }
        }
    }
}
