using FollowYourDreams.Avatar;
using UnityEngine;
using UnityEngine.AI;

namespace FollowYourDreams.AI {
    sealed class EnemyMove : MonoBehaviour {

        [SerializeField]
        NavMeshAgent agent = default;
        [SerializeField]
        AvatarController target = default;
        [SerializeField, Range(0f, 50f)]
        float lineOfSightDistance = 10.0f;

        bool targetAquired = false;

        void OnValidate() {
            if (!agent) {
                TryGetComponent(out agent);
            }
            if (!target) {
                target = FindAnyObjectByType<AvatarController>();
            }
        }

        void Update() {
            if (!targetAquired
                && Vector3.Distance(transform.position, target.transform.position) < lineOfSightDistance) {
                targetAquired = true;
            }
            if (targetAquired) {
                agent.SetDestination(target.transform.position);
            }
        }
    }
}
