using System.Collections;
using FollowYourDreams.Avatar;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace FollowYourDreams.Level {
    sealed class CookieJar : ComponentFeature<AsepriteSprite>, IInteractable {
        [SerializeField, Range(0, 10)]
        float invokeEventDelay = 1;
        [SerializeField]
        UnityEvent onInteract = new();

        public void Select() {
            observedComponent.currentSpriteId = 1;
        }
        public void Deselect() {
            observedComponent.currentSpriteId = 0;
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            avatar.currentAnimation = AvatarAnimation.Interact;
            yield return Wait.forSeconds[invokeEventDelay];
            onInteract.Invoke();
        }
    }
}
