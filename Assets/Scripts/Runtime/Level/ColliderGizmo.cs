using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class ColliderGizmo : ComponentFeature<Collider> {
#if UNITY_EDITOR
        [SerializeField]
        Color gizmoColor = Color.magenta;
        void OnDrawGizmos() {
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            switch (observedComponent) {
                case BoxCollider box:
                    Gizmos.DrawWireCube(box.center, box.size);
                    break;
                case SphereCollider sphere:
                    Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                    break;
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
#endif
    }
}
