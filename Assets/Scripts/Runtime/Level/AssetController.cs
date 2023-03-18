using System;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class AssetController : DimensionEnumController {
        [SerializeField]
        GameObject[] hideInReal = Array.Empty<GameObject>();
        [SerializeField]
        GameObject[] hideInDream = Array.Empty<GameObject>();
        [SerializeField]
        GameObject[] hideInNightmare = Array.Empty<GameObject>();
        protected override void OnSetDimension(Dimension dimension) {
            SetAllActive(hideInReal, dimension != Dimension.RealWorld);
            SetAllActive(hideInDream, dimension != Dimension.Dreamscape);
            SetAllActive(hideInNightmare, dimension != Dimension.NightmareRealm);
        }
        void SetAllActive(GameObject[] objects, bool isActive) {
            foreach (var obj in objects) {
                obj.SetActive(isActive);
            }
        }
    }
}
