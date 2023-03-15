using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    abstract class ScriptableTile : TileBase {
        [Header("Base Tile")]
        [SerializeField]
        GameObject prefab = default;
        [SerializeField]
        Color tint = Color.white;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.color = tint;
            tileData.flags = TileFlags.LockColor | TileFlags.LockTransform;
            tileData.colliderType = Tile.ColliderType.None;
            tileData.gameObject = prefab;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (go) {
                go.transform.localRotation = prefab.transform.localRotation;
                go.layer = tilemap.GetComponent<Transform>().gameObject.layer;
            }
            return true;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            tilemap.RefreshTile(position);
        }
    }
}
