using System;
using FollowYourDreams.Level;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class BedController : ComponentFeature<BedState> {
        public event Action<BedAnimation> onAnimationChange;

        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        SpriteRenderer attachedRenderer;

        public BedAnimation currentAnimation {
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

        protected override void OnValidate() {
            base.OnValidate();
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
        }

        void Update() {
            if (currentAnimation == BedAnimation.None) {
                attachedRenderer.enabled = false;
            } else {
                attachedRenderer.enabled = true;
                UpdateAnimation();
                attachedAnimator.Play(BedSettings.GetAnimationName(currentAnimation));
                attachedAnimator.Update(Time.deltaTime);
            }
        }

        void UpdateAnimation() {
            if (observedComponent.isOccupied) {
                if (currentAnimation == BedAnimation.BedEmpty) {
                    currentAnimation = BedAnimation.Sleep;
                    return;
                }
            } else {
                if (currentAnimation == BedAnimation.Sleep) {
                    currentAnimation = BedAnimation.BedEmpty;
                    return;
                }
            }
        }
    }
}
