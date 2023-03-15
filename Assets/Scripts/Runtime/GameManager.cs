using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class GameManager : ScriptableAsset {
        public static readonly float pixelsPerUnit = 16 * Mathf.Sqrt(2);

        [SerializeField, Layer]
        int realLayer;
        [SerializeField, Layer]
        int dreamLayer;
        [SerializeField, Layer]
        int nightmareLayer;

        [SerializeField]
        public Dimension currentDimension;

        public Dimension GetDimensionByLayer(int layer) {
            if (layer == realLayer) {
                return Dimension.RealWorld;
            }
            if (layer == dreamLayer) {
                return Dimension.Dreamscape;
            }
            if (layer == nightmareLayer) {
                return Dimension.NightmareRealm;
            }
            throw new ArgumentOutOfRangeException();
        }
        public int GetLayerByDimension(Dimension dimension) => dimension switch {
            Dimension.RealWorld => realLayer,
            Dimension.Dreamscape => dreamLayer,
            Dimension.NightmareRealm => nightmareLayer,
            _ => throw new NotImplementedException(),
        };

        public float realStrength => currentDimension == Dimension.RealWorld
            ? 1
            : 0;
        public float dreamStrength => currentDimension == Dimension.Dreamscape
            ? 1
            : 0;
        public float nightmareStrength => currentDimension == Dimension.NightmareRealm
            ? 1
            : 0;

        public void ActivateRealWorld() {
            currentDimension = Dimension.RealWorld;
        }
        public void ActivateDreamscape() {
            currentDimension = Dimension.Dreamscape;
        }
        public void ActivateNightmareRealm() {
            currentDimension = Dimension.NightmareRealm;
        }

#if UNITY_EDITOR
        const string PATH = "Assets/Settings/GameManager.asset";
        [UnityEditor.InitializeOnLoadMethod()]
        static void OnLoad() {
            var manager = UnityEditor.AssetDatabase.LoadAssetAtPath<GameManager>(PATH);
            manager.ActivateRealWorld();
            UnityEditor.EditorApplication.playModeStateChanged += state => {
                if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                    manager.ActivateRealWorld();
                }
            };
        }
#endif
    }
}
