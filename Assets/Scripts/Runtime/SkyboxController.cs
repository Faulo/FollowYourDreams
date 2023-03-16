using Slothsoft.UnityExtensions;
using UnityEngine.UI;

namespace FollowYourDreams {
    sealed class SkyboxController : DimensionFloatController<Image> {
        protected override float GetDimension(Image dimension) {
            return dimension.color.a;
        }
        protected override void SetDimension(Image dimension, float strength) {
            dimension.color = dimension.color.WithAlpha(strength);
        }
    }
}
