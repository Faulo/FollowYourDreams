using UnityEngine;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class GameManager : ScriptableAsset {
        public static readonly float pixelsPerUnit = 16 * Mathf.Sqrt(2);

        [SerializeField]
        public Dimension currentDimension;

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
    }
}
