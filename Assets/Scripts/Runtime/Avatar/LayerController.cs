using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    sealed class LayerController : DimensionEnumController {
        [Header("Config")]
        [SerializeField, Layer]
        int real;
        [SerializeField, Layer]
        int dream;
        [SerializeField, Layer]
        int nightmare;

        protected override void OnSetDimension(DimensionId dimension) {
            gameObject.layer = dimension switch {
                DimensionId.RealWorld => real,
                DimensionId.Dreamscape => dream,
                DimensionId.NightmareRealm => nightmare,
                _ => throw new NotImplementedException(dimension.ToString()),
            };
        }
    }
}
