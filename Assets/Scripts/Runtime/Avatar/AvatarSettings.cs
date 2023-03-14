using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class AvatarSettings : ScriptableObject {
        [SerializeField]
        GameObject prefab;

        [Header("Animations")]
#if UNITY_EDITOR
        [SerializeField]
        Texture2D sheet;
        [SerializeField]
        TextAsset json;
        [SerializeField]
        Animator animatorPrefab;

        [ContextMenu(nameof(LoadPrefab))]
        void LoadPrefab() {
            prefab.TryGetComponent(out animatorPrefab);
        }

        [ContextMenu(nameof(LoadSheet))]
        void LoadSheet() {
            LoadPrefab();
        }
#endif
    }
}
