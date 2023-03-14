using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class WallTile : TileBase {
        enum Segment {
            Grounded = 0,
            Top = 1,
            Middle = 2,
            Bottom = 3,
        }
        [SerializeField]
        GameObject prefab = default;
        [SerializeField]
        Texture2D sheet = default;
        [SerializeField]
        Sprite[] sprites = Array.Empty<Sprite>();
        [SerializeField]
        Color tint = Color.white;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = sprites[(int)CalculateSegment(position, tilemap)];
            tileData.color = tint;
            tileData.flags = TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.None;
            tileData.gameObject = prefab;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (go) {
                // go.transform.eulerAngles = new(0, 45, 0);
            }
            return true;
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
                : Segment.Grounded;
        }

#if UNITY_EDITOR
        [ContextMenu(nameof(OnValidate))]
        void OnValidate() {
            if (sheet) {
                sprites = sheet.LoadSprites();
            }
        }
#endif
    }
}
