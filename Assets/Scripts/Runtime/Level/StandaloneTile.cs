using FollowYourDreams.Avatar;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class StandaloneTile : ScriptableTile {
        [Header("Standalone Tile")]
        [SerializeField]
        Sprite sprite = default;
        [SerializeField]
        bool instantiateSpriteInRuntime = true;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
#if UNITY_EDITOR
            tileData.sprite = instantiateSpriteInRuntime || !UnityEditor.EditorApplication.isPlaying
                ? sprite
                : null;
#else
            if (instantiateSpriteInRuntime) {
                tileData.sprite = sprite;
            }
#endif
        }

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
                sprite = sheet.ExtractSprite(data, pivot, this);
            }
        }
#endif
    }
}
