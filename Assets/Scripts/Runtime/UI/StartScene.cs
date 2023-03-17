using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FollowYourDreams.UI {
    [CreateAssetMenu]
    sealed class StartScene : ScriptableAsset {
        [SerializeField]
        SceneReference scene = default;

        public void OpenSceneSingle() {
            SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Single);
        }

    }
}
