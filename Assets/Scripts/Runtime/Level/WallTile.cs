using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class WallTile : TileBase {
        enum Segment {
            Top = 0,
            Middle = 1,
            Bottom = 2,
            FreeFloating = 3,
        }
        [SerializeField]
        Texture2D sheet = default;
        [SerializeField]
        Sprite[] sprites = Array.Empty<Sprite>();

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = sprites[(int)CalculateSegment(position, tilemap)];
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            tilemap.RefreshTile(position);
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
                : Segment.FreeFloating;
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(OnValidate))]
        void OnValidate() {
            if (sheet) {
                sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(sheet))
                    .OfType<Sprite>()
                    .OrderBy(sprite => sprite.name)
                    .ToArray();
            }
        }
#endif
    }
}
