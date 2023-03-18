using System.Collections;
using FMOD.Studio;
using FMODUnity;
using FollowYourDreams.Level;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FollowYourDreams {
    static class RuntimeExtensions {
        public static EventInstance PlayOnce(this in EventReference reference) {
            if (reference.IsNull) {
                return default;
            }
            var instance = RuntimeManager.CreateInstance(reference);
            instance.start();
            return instance;
        }
        public static Coroutine PlayRepeatedly(this in EventReference reference, MonoBehaviour context, float interval) {
            return context.StartCoroutine(PlayRepeatedly_Co(reference, interval));
        }
        static IEnumerator PlayRepeatedly_Co(EventReference reference, float interval) {
            while (true) {
                var instance = RuntimeManager.CreateInstance(reference);
                instance.start();
                yield return Wait.forSeconds[interval];
            }
        }
        public static bool HasGround(this Tilemap tilemap, Vector3Int position) {
            return !tilemap.IsGround(position) && tilemap.IsGround(position + Vector3Int.back);
        }
        public static bool IsGround(this Tilemap tilemap, Vector3Int position) {
            var tile = tilemap.GetTile<ScriptableTile>(position);
            if (!tile) {
                return false;
            }
            return tile.isGround;
        }
    }
}
