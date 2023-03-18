using System;
using System.Collections;
using FMODUnity;
using FollowYourDreams.Avatar;
using MyBox;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class DoorState : MonoBehaviour, IInteractable {
        public event Action<bool> onChangeOpen;
        public event Action<bool> onChangeSelect;

        [Header("Config")]
        [SerializeField]
        GameObject isOpenCollider;
        [SerializeField]
        GameObject isClosedCollider;
        [SerializeField, Range(0, 10)]
        float interactDuration = 0.2f;

        [Header("Audio")]
        [SerializeField]
        EventReference toggleEvent = new();

        bool isOpen {
            get => m_isOpen;
            set {
                m_isOpen = value;
                onChangeOpen?.Invoke(value);
                toggleEvent.PlayOnce();
            }
        }
        [Header("Runtime")]
        [SerializeField, ReadOnly]
        bool m_isOpen;

        public int priority => 0;
        public bool isSelectable => !isOpen;

        void Start() {
            onChangeOpen += isOpen => {
                isOpenCollider.SetActive(isOpen);
                isClosedCollider.SetActive(!isOpen);
            };
            isOpen = false;
        }

        [ContextMenu(nameof(ToggleOpen))]
        public void ToggleOpen() {
            isOpen = !isOpen;
        }

        public void Select() {
            onChangeSelect?.Invoke(true);
        }
        public void Deselect() {
            onChangeSelect?.Invoke(false);
        }
        public IEnumerator Interact_Co(AvatarController avatar) {
            avatar.currentAnimation = AvatarAnimation.Interact;

            yield return Wait.forSeconds[interactDuration];

            ToggleOpen();

            yield return Wait.forSeconds[interactDuration];

            avatar.currentAnimation = AvatarAnimation.Idle;
        }
    }
}
