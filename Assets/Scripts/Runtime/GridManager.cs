using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams {
    sealed class GridManager : ComponentFeature<Grid> {
        public Tilemap this[Dimension dimension] {
            get => dimension switch {
                Dimension.RealWorld => real,
                Dimension.Dreamscape => dream,
                Dimension.NightmareRealm => nightmare,
                _ => throw new System.NotImplementedException(),
            };
        }

        public static GridManager instance;

        [SerializeField]
        GameManager manager;

        [Header("Dimensions")]
        [SerializeField]
        Tilemap real;
        [SerializeField]
        Tilemap dream;
        [SerializeField]
        Tilemap nightmare;

        void Awake() {
            instance = this;
        }

        public Vector3 RoundToTileCenter(in Vector3 position3D, Dimension dimension = Dimension.RealWorld) {
            var tilemap = this[dimension];
            var gridPosition = tilemap.WorldToCell(position3D);
            return tilemap.GetCellCenterWorld(gridPosition);
        }

        public Vector3 RoundToTileCenterGround(in Vector3 position3D, Dimension dimension = Dimension.RealWorld) {
            var tilemap = this[dimension];
            var gridPosition = real.WorldToCell(position3D);

            if (tilemap.HasGround(gridPosition)) {
                return tilemap.GetCellCenterWorld(gridPosition);
            }

            for (int i = 0; i < 10; i++) {
                var testPosition = gridPosition;
                testPosition.z += i;
                if (tilemap.HasGround(testPosition)) {
                    return tilemap.GetCellCenterWorld(testPosition);
                }
                testPosition.z -= i;
                testPosition.z -= i;
                if (tilemap.HasGround(testPosition)) {
                    return tilemap.GetCellCenterWorld(testPosition);
                }
            }

            return tilemap.GetCellCenterWorld(gridPosition);
        }
        public Vector3 RoundToTileCenterGround(in Vector3 position3D, int layer) {
            var dimension = manager.GetDimensionByLayer(layer);
            return RoundToTileCenterGround(position3D, dimension);
        }
    }
}
