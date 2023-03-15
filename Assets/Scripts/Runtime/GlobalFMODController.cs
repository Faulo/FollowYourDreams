using FMODUnity;

namespace FollowYourDreams {
    sealed class GlobalFMODController : DimensionEnumController {
        protected override void OnSetDimension(Dimension dimension) {
            if (RuntimeManager.IsInitialized) {
                RuntimeManager.StudioSystem.setParameterByNameWithLabel(nameof(Dimension), dimension.ToString());
            }
        }
    }
}
