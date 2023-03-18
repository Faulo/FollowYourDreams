using UnityEngine;

namespace FollowYourDreams.Level {
    [CreateAssetMenu]
    class StandalonePlantTile : StandaloneEnumTile<StandalonePlantTile.PlantType> {
        internal enum PlantType {
            RealWorld,
            Dreamscape,
            NightmareRealm,
            Revert,
            Small,
            FrizzyLeft,
            FrizzyRight
        }
    }
}
