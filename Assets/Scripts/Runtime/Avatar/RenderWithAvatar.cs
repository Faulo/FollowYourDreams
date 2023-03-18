using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class RenderWithAvatar : ComponentFeature<AvatarController> {
        [SerializeField]
        SpriteRenderer attachedRenderer;

        protected override void OnValidate() {
            base.OnValidate();
            if (!attachedRenderer) {
                TryGetComponent(out attachedRenderer);
            }
        }

        void OnEnable() {
            observedComponent.onAnimationChange += OnAnimationChange;
        }
        void OnDisable() {
            observedComponent.onAnimationChange -= OnAnimationChange;
        }

        void OnAnimationChange(AvatarAnimation animation) {
            attachedRenderer.enabled = animation != AvatarAnimation.None;
        }
    }
}
