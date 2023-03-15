using UnityEngine;

namespace FollowYourDreams {
    sealed class MaterialController : DimensionController<Material> {
        [SerializeField]
        string alphaName = "_Alpha";

        protected override float GetDimension(Material dimension) {
            return dimension.GetFloat(alphaName);
        }
        protected override void SetDimension(Material dimension, float strength) {
            dimension.SetFloat(alphaName, strength);
        }
    }
}
