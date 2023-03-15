using UnityEngine.Rendering;

namespace FollowYourDreams {
    sealed class VolumeController : DimensionFloatController<Volume> {
        protected override float GetDimension(Volume dimension) {
            return dimension.weight;
        }
        protected override void SetDimension(Volume dimension, float strength) {
            dimension.weight = strength;
        }
    }
}
