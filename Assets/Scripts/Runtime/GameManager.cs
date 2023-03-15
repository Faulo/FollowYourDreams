using UnityEngine;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class GameManager : ScriptableAsset {
        public static readonly float pixelsPerUnit = 16 * Mathf.Sqrt(2);

        [SerializeField]
        public DimensionId currentDimension;

        public float realStrength => currentDimension == DimensionId.RealWorld
            ? 1
            : 0;
        public float dreamStrength => currentDimension == DimensionId.Dreamscape
            ? 1
            : 0;
        public float nightmareStrength => currentDimension == DimensionId.NightmareRealm
            ? 1
            : 0;

        public void ActivateRealWorld() {
            currentDimension = DimensionId.RealWorld;
        }
        public void ActivateDreamscape() {
            currentDimension = DimensionId.Dreamscape;
        }
        public void ActivateNightmareRealm() {
            currentDimension = DimensionId.NightmareRealm;
        }
    }
}
