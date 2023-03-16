using FollowYourDreams.Avatar;
using UnityEngine;

namespace FollowYourDreams {
    sealed class AsepriteSprite : ComponentFeature<SpriteRenderer> {
#if UNITY_EDITOR
        [Header("Editor-only")]
        [SerializeField]
        TextAsset json = default;
        [SerializeField]
        Texture2D sheet = default;
        [SerializeField]
        Vector2 pivot = new(0.5f, 0.5f);
        [SerializeField]
        AsepriteData data = new();

        [ContextMenu(nameof(LoadSprite))]
        public void LoadSprite() {
            if (sheet && json) {
                data = AsepriteData.FromJson(json.text);
                observedComponent.sprite = sheet.ExtractSprite(data, pivot, gameObject);
            }
        }
#endif
    }
}
