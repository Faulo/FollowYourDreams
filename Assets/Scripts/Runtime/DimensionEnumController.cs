using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams {
    [ExecuteAlways]
    abstract class DimensionEnumController : MonoBehaviour {
        [Header("Setup")]
        [SerializeField, Expandable]
        GameManager manager;

        Dimension previousDimension;

        [ContextMenu(nameof(Start))]
        protected void Start() {
            if (manager) {
                previousDimension = manager.currentDimension;
                OnSetDimension(previousDimension);
            }
        }

        protected void Update() {
            if (manager) {
                if (previousDimension != manager.currentDimension) {
                    previousDimension = manager.currentDimension;
                    OnSetDimension(previousDimension);
                }
            }
        }

        protected abstract void OnSetDimension(Dimension dimension);
    }
}
