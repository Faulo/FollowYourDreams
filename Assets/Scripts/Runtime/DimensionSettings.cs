using UnityEngine;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class DimensionSettings : ScriptableAsset {
        [SerializeField]
        Material tilemapMaterial = default;
    }
}
