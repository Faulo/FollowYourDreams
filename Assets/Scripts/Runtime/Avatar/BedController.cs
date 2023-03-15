using FollowYourDreams.Level;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class BedController : ComponentFeature<BedState> {
        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        SpriteRenderer attachedRenderer;

        public BedAnimation currentAnimation {
            get => AnimatorExtensions<BedAnimation>.GetCurrentAnimation(attachedAnimator);
            set => attachedAnimator.Play(value.ToString());
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
        }

        void OnEnable() {
            observedComponent.onSetAnimation += OnSetAnimation;
        }

        void OnDisable() {
            observedComponent.onSetAnimation += OnSetAnimation;
        }

        void OnSetAnimation(BedAnimation animation) {
            currentAnimation = animation;
        }

        void Update() {
            if (currentAnimation == BedAnimation.None) {
                attachedRenderer.enabled = false;
            } else {
                attachedRenderer.enabled = true;
                attachedAnimator.Update(Time.deltaTime);
            }
        }
    }
}
