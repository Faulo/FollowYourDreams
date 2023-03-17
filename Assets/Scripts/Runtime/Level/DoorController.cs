using UnityEngine;

namespace FollowYourDreams.Level {
    [ExecuteAlways]
    sealed class DoorController : ComponentFeature<DoorState> {
        [Header("Configuration")]
        [SerializeField]
        AsepriteSprite attachedSprite;

        protected override void OnValidate() {
            base.OnValidate();
            if (!attachedSprite) {
                TryGetComponent(out attachedSprite);
            }
        }
        void OnEnable() {
            observedComponent.onChangeOpen += HandleOpen;
        }
        void OnDisable() {
            observedComponent.onChangeOpen += HandleOpen;
        }

        void HandleOpen(bool isOpen) {
            attachedSprite.currentSpriteId = isOpen
                ? 0
                : 1;
        }
    }
}
