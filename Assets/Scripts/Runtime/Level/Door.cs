using UnityEngine;

namespace FollowYourDreams.Level {
    [ExecuteAlways]
    sealed class Door : ComponentFeature<AsepriteSprite> {
        [SerializeField]
        bool isTopRight = false;

        protected override void OnValidate() {
            base.OnValidate();
            Update();
        }
        void Update() {
            observedComponent.currentSpriteId = isTopRight
                ? 1
                : 0;
        }
    }
}
