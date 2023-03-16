using System.Collections;
using FMODUnity;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams {
    static class RuntimeExtensions {
        public static void PlayOnce(this in EventReference reference) {
            var instance = RuntimeManager.CreateInstance(reference);
            instance.start();
        }
        public static Coroutine PlayRepeatedly(this in EventReference reference, MonoBehaviour context, float interval) {
            return context.StartCoroutine(PlayRepeatedly_Co(reference, interval));
        }
        static IEnumerator PlayRepeatedly_Co(EventReference reference, float interval) {
            while (true) {
                var instance = RuntimeManager.CreateInstance(reference);
                instance.start();
                yield return Wait.forSeconds[interval];
            }
        }
    }
}
