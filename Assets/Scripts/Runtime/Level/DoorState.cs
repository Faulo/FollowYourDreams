using System;
using MyBox;
using UnityEngine;

namespace FollowYourDreams.Level {
    sealed class DoorState : MonoBehaviour {
        public event Action<bool> onChangeOpen;

        [Header("Config")]
        [SerializeField]
        GameObject isOpenCollider;
        [SerializeField]
        GameObject isClosedCollider;

        [Header("Runtime")]
        [SerializeField, ReadOnly]
        bool isOpen = false;

        void Start() {
            onChangeOpen += isOpen => {
                isOpenCollider.SetActive(isOpen);
                isClosedCollider.SetActive(!isOpen);
            };
            onChangeOpen?.Invoke(isOpen);
        }

        [ContextMenu(nameof(ToggleOpen))]
        public void ToggleOpen() {
            isOpen = !isOpen;
            onChangeOpen?.Invoke(isOpen);
        }
    }
}
