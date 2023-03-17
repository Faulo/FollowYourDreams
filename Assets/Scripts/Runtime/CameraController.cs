using UnityEngine;

namespace FollowYourDreams {
    [ExecuteAlways]
    sealed class CameraController : ComponentFeature<Camera> {
        [SerializeField]
        LayerMask runtimeCulling = ~0;

#if UNITY_EDITOR
        [SerializeField]
        LayerMask editorCulling = ~0;

        void Update() {
            observedComponent.cullingMask = Application.isPlaying
                ? runtimeCulling
                : editorCulling;
        }
#else
        void Start() {
            observedComponent.cullingMask = runtimeCulling;
        }
#endif
    }
}
