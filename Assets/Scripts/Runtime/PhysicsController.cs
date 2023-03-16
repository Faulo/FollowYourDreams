using System;
using UnityEngine;

namespace FollowYourDreams {
    sealed class PhysicsController : DimensionEnumController {
        [SerializeField, Range(0, 100)]
        float realGravity = 10;
        [SerializeField, Range(0, 100)]
        float dreamGravity = 10;
        [SerializeField, Range(0, 100)]
        float nightmareGravity = 10;

        protected override void OnSetDimension(Dimension dimension) {
            float gravity = dimension switch {
                Dimension.RealWorld => realGravity,
                Dimension.Dreamscape => dreamGravity,
                Dimension.NightmareRealm => nightmareGravity,
                _ => throw new NotImplementedException(),
            };
            Physics.gravity = new(0, -gravity, 0);
        }
    }
}
