namespace FollowYourDreams.Avatar {
    sealed class AvatarDeathZone : ComponentFeature<AvatarController> {
        void FixedUpdate() {
            if (observedComponent.currentAnimation != AvatarAnimation.None && transform.position.y < 0) {
                observedComponent.Die();
            }
        }
    }
}
