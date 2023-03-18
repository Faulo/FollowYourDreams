using System;
using System.Collections.Generic;
using System.Linq;
using FollowYourDreams.Graphics;
using Slothsoft.UnityExtensions.Editor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class SpriteManager : ScriptableAsset {
#if UNITY_EDITOR
        [SerializeField]
        ScriptableObject[] assets = Array.Empty<ScriptableObject>();
        [SerializeField]
        Component[] components = Array.Empty<Component>();

        IEnumerable<IImportable> importables => assets
            .Cast<IImportable>()
            .Union(components.Cast<IImportable>());

        [ContextMenu(nameof(LoadObjects))]
        public void LoadObjects() {
            assets = AssetUtils
                .LoadAssetsOfType<ScriptableObject>()
                .Where(obj => obj is IImportable)
                .ToArray();

            components = PrefabUtils
                .allPrefabs
                .SelectMany(obj => obj.GetComponents<IImportable>())
                .OfType<Component>()
                .ToArray();
        }

        [ContextMenu(nameof(LoadSpriteAssets))]
        public void LoadSpriteAssets() {
            foreach (var asset in assets) {
                if (asset is IImportable importable) {
                    Debug.Log($"Loading sprite of: {asset}");
                    importable.LoadAll();
                }
            }
        }

        [ContextMenu(nameof(LoadSpriteAssets))]
        public void LoadSpriteComponents() {
            foreach (var component in components) {
                if (component is IImportable importable) {
                    Debug.Log($"Loading sprite of: {component}");
                    importable.LoadAll();
                }
            }
        }

        [SerializeField]
        Vector2 pivot;
        [ContextMenu(nameof(SetAllPivots))]
        public void SetAllPivots() {
            foreach (var importable in importables) {
                if (importable.spritePivot != pivot) {
                    Debug.Log($"Setting pivot of: {importable as UnityObject} from {importable.spritePivot} to {pivot}");
                    importable.spritePivot = pivot;
                }
            }
        }
        [ContextMenu(nameof(LoadAllSprites))]
        public void LoadAllSprites() {
            foreach (var importable in importables) {
                importable.LoadAll();
            }
        }
#endif
    }
}
