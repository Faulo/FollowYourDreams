using UnityEngine;

namespace FollowYourDreams.Level {
    [ExecuteAlways]
    sealed class DoorController : ComponentFeature<DoorState> {
        [Header("Configuration")]
        [SerializeField]
        Animator attachedAnimator;
        [SerializeField]
        Dimension dimension;
        [SerializeField]
        bool isOpen;
        [SerializeField]
        bool isSelected;

        DoorSprite doorState {
            set {
                attachedAnimator.Play(DoorSettings.GetAnimationName(value));
            }
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (!attachedAnimator) {
                TryGetComponent(out attachedAnimator);
            }
        }
        void Start() {
            observedComponent.onChangeOpen += HandleOpen;
            observedComponent.onChangeSelect += HandleSelect;
        }
        void OnDestroy() {
            observedComponent.onChangeOpen -= HandleOpen;
            observedComponent.onChangeSelect -= HandleSelect;
        }
        void OnEnable() {
            UpdateState();
        }

        void HandleOpen(bool isOpen) {
            this.isOpen = isOpen;
            UpdateState();
        }

        void HandleSelect(bool isSelected) {
            this.isSelected = isSelected;
            UpdateState();
        }

        void UpdateState() {
            doorState = dimension switch {
                Dimension.RealWorld when isOpen && isSelected => DoorSprite.NormalDoorOpenSelected,
                Dimension.RealWorld when isOpen => DoorSprite.NormalDoorOpen,
                Dimension.RealWorld when isSelected => DoorSprite.NormalDoorClosedSelected,
                Dimension.RealWorld => DoorSprite.NormalDoorClosed,

                Dimension.Dreamscape when isOpen && isSelected => DoorSprite.DreamDoorOpenSelected,
                Dimension.Dreamscape when isOpen => DoorSprite.DreamDoorOpen,
                Dimension.Dreamscape when isSelected => DoorSprite.DreamDoorClosedSelected,
                Dimension.Dreamscape => DoorSprite.DreamDoorClosed,

                Dimension.NightmareRealm when isOpen && isSelected => DoorSprite.NightmareDoorOpenSelected,
                Dimension.NightmareRealm when isOpen => DoorSprite.NightmareDoorOpen,
                Dimension.NightmareRealm when isSelected => DoorSprite.NightmareDoorClosedSelected,
                Dimension.NightmareRealm => DoorSprite.NightmareDoorClosed,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
