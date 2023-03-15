namespace FollowYourDreams.Avatar {
    sealed class LayerController : DimensionEnumController {
        protected override void OnSetDimension(Dimension dimension) {
            gameObject.layer = manager.GetLayerByDimension(dimension);
        }
    }
}
