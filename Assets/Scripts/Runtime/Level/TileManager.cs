using System.Linq;
using FollowYourDreams.Graphics;
using Slothsoft.UnityExtensions.Editor;
using UnityEngine;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    sealed class TileManager : ScriptableAsset {
#if UNITY_EDITOR
        [SerializeField]
        ScriptableTile[] tiles;

        [ContextMenu(nameof(LoadTiles))]
        public void LoadTiles() {
            tiles = AssetUtils
                .LoadAssetsOfType<ScriptableTile>()
                .ToArray();
        }

        [ContextMenu(nameof(LoadSprites))]
        public void LoadSprites() {
            foreach (var tile in tiles) {
                if (tile is IImportable importer) {
                    Debug.Log($"Loading sprite of: {tile}");
                    importer.LoadSprite();
                }
            }
        }
#endif
    }
}
