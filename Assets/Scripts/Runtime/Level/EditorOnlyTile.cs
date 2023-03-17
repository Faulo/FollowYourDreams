using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class EditorOnlyTile : ScriptableTile {
#if UNITY_EDITOR
        [SerializeField]
        GameObject editorPrefab;
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.gameObject = editorPrefab;
        }
#endif
    }
}
