using TMPro;
using UnityEngine;

namespace FollowYourDreams.UI {
    [ExecuteAlways]
    sealed class VersionText : ComponentFeature<TextMeshProUGUI> {
        [SerializeField]
        string template = "productName vversion";

        void Update() {
            observedComponent.text = template
                .Replace(nameof(Application.version), Application.version)
                .Replace(nameof(Application.productName), Application.productName);
        }
    }
}
