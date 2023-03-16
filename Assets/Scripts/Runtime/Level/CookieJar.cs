using System.Collections;
using FollowYourDreams.Avatar;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class CookieJar : MonoBehaviour, IInteractable {
        public void Select() {
        }
        public void Deselect() {
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            yield return null;
            Debug.Log("YOU WIN");
        }
    }
}
