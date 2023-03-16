using System.Collections;
using FollowYourDreams.Avatar;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class CookieJar : ComponentFeature<AsepriteSprite>, IInteractable {
        public void Select() {
            observedComponent.currentSpriteId = 1;
        }
        public void Deselect() {
            observedComponent.currentSpriteId = 0;
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            yield return null;
            Debug.Log("YOU WIN");
        }
    }
}
