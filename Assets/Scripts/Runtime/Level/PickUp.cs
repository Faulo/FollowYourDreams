using System;
using System.Collections;
using FollowYourDreams.Avatar;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class PickUp : ComponentFeature<AsepriteSprite>, IInteractable, ICarryable {
        [SerializeField]
        Transform context;
        [SerializeField, Range(0, 10)]
        float interactDuration = 0.1f;
        [SerializeField]
        AvatarController follow;
        bool isPickedUp => follow;

        [SerializeField]
        int defaultSpriteId = 0;
        [SerializeField]
        int selectedSpriteId = 1;

        protected override void OnValidate() {
            base.OnValidate();
            Deselect();
        }
        void Start() {
            Deselect();
        }
        public int priority => 1;
        public bool isSelectable => true;

        public Vector3 position {
            get => context.transform.position;
            set => context.transform.position = value;
        }

        public void Select() {
            observedComponent.currentSpriteId = selectedSpriteId;
        }
        public void Deselect() {
            observedComponent.currentSpriteId = defaultSpriteId;
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            avatar.currentAnimation = AvatarAnimation.Push;
            if (isPickedUp) {
                follow = null;
                avatar.carryable = null;
            } else {
                follow = avatar;
                avatar.carryable = this;
            }
            yield return Wait.forSeconds[interactDuration];
        }
        void LateUpdate() {
            if (isPickedUp) {
                observedComponent.currentSpriteId = defaultSpriteId;
            }
        }
    }
}
