using System;
using System.Linq;
using UnityEngine;

namespace FollowYourDreams {
    static class AnimatorExtensions<T> where T : Enum {
        static readonly (T value, string name)[] animations = Enum
            .GetValues(typeof(T))
            .Cast<T>()
            .Select(anim => (anim, anim.ToString()))
            .ToArray();

        public static T GetCurrentAnimation(in Animator animator) {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            for (int i = 0; i < animations.Length; i++) {
                if (state.IsName(animations[i].name)) {
                    return animations[i].value;
                }
            }
            return default;
        }
    }
}
