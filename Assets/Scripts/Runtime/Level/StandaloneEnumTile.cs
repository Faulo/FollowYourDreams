using UnityEngine;

namespace FollowYourDreams.Level {
    abstract class StandaloneEnumTile<TEnum> : StandaloneTile where TEnum : struct {
        [SerializeField]
        TEnum index;

        protected override void OnValidate() {
            base.OnValidate();

            spriteIndex = (int)(object)index;
        }
    }
}
