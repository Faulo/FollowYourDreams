using UnityEngine;

namespace FollowYourDreams.UI {
    [CreateAssetMenu]
    sealed class QuitGame : ScriptableAsset {
        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
