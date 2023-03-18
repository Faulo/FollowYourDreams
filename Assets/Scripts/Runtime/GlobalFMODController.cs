using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FollowYourDreams {
    sealed class GlobalFMODController : DimensionEnumController {
        [SerializeField]
        EventReference musicEvent = new();

        EventInstance instance;

        void OnEnable() {
            if (Application.isPlaying) {
                instance = musicEvent.PlayOnce();
            }
        }
        void OnDisable() {
            if (instance.isValid()) {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        protected override void OnSetDimension(Dimension dimension) {
            if (RuntimeManager.IsInitialized) {
                RuntimeManager.StudioSystem.setParameterByNameWithLabel(nameof(Dimension), dimension.ToString());
            }
        }
    }
}
