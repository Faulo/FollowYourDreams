using System;
using UnityEngine;

namespace FollowYourDreams {
    [CreateAssetMenu]
    sealed class GameManager : ScriptableAsset {
        public event Action<DimensionId> onSetDimension;

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

        protected override void OnValidate() {
            base.OnValidate();
            onSetDimension?.Invoke(currentDimension);
        }
    }
}
