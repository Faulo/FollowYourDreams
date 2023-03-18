using System;
using System.Collections;
using FollowYourDreams.Avatar;
using UnityEngine;
using UnityEngine.AI;

namespace FollowYourDreams.AI {
    sealed class EnemyBehavior : MonoBehaviour {

        [SerializeField]
        NavMeshAgent agent = default;
        [SerializeField]
        AvatarController target = default;
        [SerializeField, Range(0f, 50f)]
        float lineOfSightDistance = 10.0f;
        [SerializeField, Range(0f, 5f)]
        float snackRange = 1f;

        bool targetAquired = false;
        bool snacked = false;

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
            if (targetAquired && !snacked) {
                agent.SetDestination(target.transform.position);
            }
            if (targetAquired && Vector3.Distance(transform.position, target.transform.position) < snackRange) {
                EatAlice();
            }
        }

        void EatAlice() {
            snacked = true;
            agent.SetDestination(transform.position);
            // TODO: 1. Play eat animation
            // 2. Check again if in range
            // a) if not: snacked = false;
            // b) if yes, then: 
            target.Die();
            // TODO: wait a bit
            StartCoroutine(WaitForAnimation());
        }

        IEnumerator WaitForAnimation() {
            yield return new WaitForSeconds(2f);
            transform.localPosition = Vector3.zero;
        }
    }
}
