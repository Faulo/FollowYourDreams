using System;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class BedController : MonoBehaviour {
        public event Action<BedAnimation> onAnimationChange;

        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;

        BedAnimation currentAnimation {
            get => m_currentAnimation;
            set {
                if (m_currentAnimation != value) {
                    m_currentAnimation = value;
                    onAnimationChange?.Invoke(value);
                }
            }
        }
        [Header("Runtime")]
        [SerializeField]
        BedAnimation m_currentAnimation = BedAnimation.BedEmpty;

        void OnValidate() {
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
        }

        void Update() {
            attachedAnimator.Play(BedSettings.GetAnimationName(currentAnimation));
            attachedAnimator.Update(Time.deltaTime);
        }
    }
}
