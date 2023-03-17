using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams {
    [ExecuteAlways]
    sealed class DebugController : MonoBehaviour {
        [SerializeField]
        Material debugMaterial;
        [SerializeField]
        string colorName = "_MainColor";
        [SerializeField]
        Color editorColor = Color.black.WithAlpha(0.1f);

#if UNITY_EDITOR
        [SerializeField]
        Color runtimeColor = Color.black.WithAlpha(0);

        void Update() {
            debugMaterial.SetColor(colorName, Application.isPlaying ? runtimeColor : editorColor);
        }
#else
        void Awake() {
            debugMaterial.SetColor(colorName, runtimeColor);
        }
#endif
    }
}
