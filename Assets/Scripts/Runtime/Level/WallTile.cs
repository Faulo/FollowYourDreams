using System.Collections.Generic;
using FollowYourDreams.Avatar;
using MyBox;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class WallTile : ScriptableTile {
        enum Segment {
            Grounded = 0,
            Top = 1,
            Middle = 2,
            Bottom = 3,
        }
        [Header("Wall Tile")]
        [SerializeField, ReadOnly]
        List<Sprite> sprites = new();

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprites[(int)CalculateSegment(position, tilemap)];
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            base.RefreshTile(position, tilemap);
            tilemap.RefreshTile(position + Vector3Int.forward);
            tilemap.RefreshTile(position + Vector3Int.back);
        }

        Segment CalculateSegment(in Vector3Int position, ITilemap tilemap) {
            var top = tilemap.GetTile(position + Vector3Int.forward);
            var bottom = tilemap.GetTile(position + Vector3Int.back);
            if (top == this) {
                return bottom == this
                    ? Segment.Middle
                    : Segment.Bottom;
            }
            return bottom == this
                ? Segment.Top
                : Segment.Grounded;
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
                sheet.ExtractSprites(data, pivot, this, sprites);
            }
        }
#endif
    }
}
