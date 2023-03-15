using System;
using System.Collections;
using FollowYourDreams.Avatar;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class BedState : MonoBehaviour, IInteractable {
        public event Action<BedAnimation> onSetAnimation;

        [Header("Config")]
        [SerializeField, Expandable]
        GameManager manager;
        [SerializeField, Range(0, 10)]
        float gotoSleepStart = 1;
        [SerializeField, Range(0, 10)]
        float gotoSleepMiddle = 1;
        [SerializeField, Range(0, 10)]
        float gotoSleepEnd = 1;
        [SerializeField, Range(0, 10)]
        float dreamAbortStart = 1;
        [SerializeField, Range(0, 10)]
        float dreamAbortMiddle = 1;
        [SerializeField, Range(0, 10)]
        float dreamAbortEnd = 1;

        [Header("Runtime")]
        [SerializeField]
        bool isOccupied = false;

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
            bool gotoSleep = !isOccupied;
            avatar.currentAnimation = AvatarAnimation.None;

            onSetAnimation?.Invoke(gotoSleep ? BedAnimation.GoToSleep : BedAnimation.DreamAbort);
            yield return Wait.forSeconds[gotoSleep ? gotoSleepStart : dreamAbortStart];

            manager.currentDimension = targetDimension;
            isOccupied = !isOccupied;
            yield return Wait.forSeconds[gotoSleep ? gotoSleepMiddle : dreamAbortMiddle];

            onSetAnimation?.Invoke(gotoSleep ? BedAnimation.DreamUp : BedAnimation.WakeUp);
            yield return Wait.forSeconds[gotoSleep ? gotoSleepEnd : dreamAbortEnd];

            avatar.currentAnimation = AvatarAnimation.Idle;
            onSetAnimation?.Invoke(gotoSleep ? BedAnimation.DreamSleep : BedAnimation.BedEmpty);
        }
    }
}
