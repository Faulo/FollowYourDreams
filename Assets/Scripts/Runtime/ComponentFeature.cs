using MyBox;
using UnityEngine;

namespace FollowYourDreams {
    abstract class ComponentFeature<T> : MonoBehaviour where T : MonoBehaviour {
        [SerializeField, ReadOnly]
        protected T observedComponent = default;

        protected virtual void OnValidate() {
            if (!observedComponent) {
                TryGetComponent(out observedComponent);
            }
        }
    }
}
