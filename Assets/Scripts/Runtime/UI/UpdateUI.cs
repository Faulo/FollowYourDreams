using FollowYourDreams.Avatar;
using UnityEngine;

namespace FollowYourDreams.UI {
    sealed class UpdateUI : MonoBehaviour {
        [SerializeField]
        AvatarSettings avatarSettings = default;
        [SerializeField]
        GameObject highJump = default;
        [SerializeField]
        GameObject glide = default;
        [SerializeField]
        GameObject climb = default;

        void Update() {
            highJump.SetActive(avatarSettings.HasPower(Power.HighJump));
            glide.SetActive(avatarSettings.HasPower(Power.Glide));
            climb.SetActive(avatarSettings.HasPower(Power.Climb));
        }
    }
}
