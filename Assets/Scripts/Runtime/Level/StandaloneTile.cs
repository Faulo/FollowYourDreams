using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class StandaloneTile : TileBase {
        [SerializeField]
        Sprite sprite = default;
        [SerializeField]
        Color tint = Color.white;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = sprite;
            tileData.color = tint;
            tileData.flags = TileFlags.LockColor;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            tilemap.RefreshTile(position);
        }
    }
}
