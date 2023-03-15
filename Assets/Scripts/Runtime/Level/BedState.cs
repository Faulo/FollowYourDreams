using System;
using System.Collections;
using FollowYourDreams.Avatar;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class BedState : MonoBehaviour, IInteractable {
        [Header("Config")]
        [SerializeField, Expandable]
        GameManager manager;
        [SerializeField, Range(0, 10)]
        float sleepDelay = 1;
        [SerializeField, Range(0, 10)]
        float wakeDelay = 1;

        [Header("Runtime")]
        [SerializeField]
        public bool isOccupied = false;

        Dimension ownDimension => manager.currentDimension;
        int dimensionOffset => isOccupied
            ? -1
            : 1;
        Dimension targetDimension => ownDimension + dimensionOffset;

        public void Select() {
        }
        public void Deselect() {
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            avatar.currentAnimation = AvatarAnimation.None;
            yield return Wait.forSeconds[sleepDelay];
            manager.currentDimension = targetDimension;
            isOccupied = !isOccupied;
            yield return Wait.forSeconds[wakeDelay];
            avatar.currentAnimation = AvatarAnimation.Idle;
        }
    }
}
