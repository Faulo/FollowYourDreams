using System.Collections.Generic;
using FollowYourDreams.Graphics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class StandaloneTile : ScriptableTile, IImportable {
        [Header("Standalone Tile")]
        [SerializeField]
        List<Sprite> sprites = new();
        [SerializeField]
        int spriteIndex = 0;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprites.Count > spriteIndex
                ? sprites[spriteIndex]
                : default;
        }

#if UNITY_EDITOR
        [Header("Editor-only")]
        [SerializeField]
        TextAsset json = default;
        [SerializeField]
        Texture2D sheet = default;
        [SerializeField]
        Vector2 pivot = new(0.5f, 0.5f);
        public Vector2 spritePivot {
            get => pivot;
            set => pivot = value;
        }
        [SerializeField]
        AsepriteData data = new();

        [ContextMenu(nameof(LoadAll))]
        public void LoadAll() {
            if (sheet && json) {
                data = AsepriteData.FromJson(json.text);
                sheet.ExtractSprites(data, pivot, this, sprites);
            }
        }
#endif
    }
}
