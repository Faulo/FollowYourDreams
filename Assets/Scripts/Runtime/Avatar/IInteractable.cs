using System.Collections;

namespace FollowYourDreams.Avatar {
    interface IInteractable {
        int priority { get; }
        bool isSelectable { get; }
        void Select();
        void Deselect();
        IEnumerator Interact_Co(AvatarController avatar);
    }
}
