using System.Collections;

namespace FollowYourDreams.Avatar {
    interface IInteractable {
        void Select();
        void Deselect();
        IEnumerator Interact_Co(AvatarController avatar);
    }
}
