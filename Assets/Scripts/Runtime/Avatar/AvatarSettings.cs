using System;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class AvatarSettings : ScriptableAsset {
        [SerializeField]
        GameObject prefab;

#if UNITY_EDITOR
        [Header("Animations")]
        [SerializeField]
        Texture2D sheet;
        [SerializeField]
        Sprite[] sprites = Array.Empty<Sprite>();
        [SerializeField]
        TextAsset json;
        [SerializeField]
        AsepriteData data = new();
        [SerializeField]
        Animator animatorPrefab;

        [ContextMenu(nameof(LoadPrefab))]
        void LoadPrefab() {
            prefab.TryGetComponent(out animatorPrefab);
        }

        [ContextMenu(nameof(LoadSheet))]
        void LoadSheet() {
            LoadPrefab();

            sprites = sheet.LoadSprites();

            data = AsepriteData.FromJson(json.text);

        }
#endif
    }
}
