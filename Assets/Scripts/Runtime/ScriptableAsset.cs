using MyBox;
using UnityEngine;

namespace FollowYourDreams {
    class ScriptableAsset : ScriptableObject {
        [SerializeField, ReadOnly]
        ScriptableAsset asset;

        protected virtual void OnValidate() {
            if (asset != this) {
                asset = this;
            }
        }
    }
}
