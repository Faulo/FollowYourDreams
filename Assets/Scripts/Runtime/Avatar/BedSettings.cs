using System;
using System.Collections.Generic;
using MyBox;
using Slothsoft.UnityExtensions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class BedSettings : ScriptableAsset {
        [Header("Setup")]
        [SerializeField]
        GameObject prefab;

#if UNITY_EDITOR
        const int DIRECTION_COUNT = 5;
        const float TIME_MULTIPLIER = 0.001f;
        [Header("Editor-only")]
        [SerializeField]
        AnimatorController controller;

        [SerializeField]
        TextAsset json;
        [SerializeField]
        Texture2D sheet;
        [SerializeField]
        Vector2 pivot = new(0.5f, 0.5f);
        [SerializeField]
        SerializableKeyValuePairs<BedAnimation, bool> isLoopingOverride = new();

        [Header("Auto-filled")]
        [SerializeField, ReadOnly]
        List<Sprite> sprites = new();
        Sprite GetSprite(int index) {
            return sprites[index];
        }
        [SerializeField, ReadOnly]
        AsepriteData data = new();
        [SerializeField, ReadOnly]
        Animator animatorPrefab;
        [SerializeField, ReadOnly]
        SpriteRenderer rendererPrefab;

        [ContextMenu(nameof(LoadPrefab))]
        void LoadPrefab() {
            prefab.TryGetComponent(out animatorPrefab);
            prefab.TryGetComponent(out rendererPrefab);
        }

        [ContextMenu(nameof(LoadData))]
        void LoadData() {
            data = AsepriteData.FromJson(json.text);
        }

        [ContextMenu(nameof(LoadSprites))]
        void LoadSprites() {
            sheet.ExtractSprites(data, pivot, this, sprites);

            if (rendererPrefab) {
                rendererPrefab.sprite = GetSprite(0);
                EditorUtility.SetDirty(rendererPrefab);
            }

            AssetDatabase.SaveAssets();
        }

        [ContextMenu(nameof(LoadController))]
        void LoadController() {
            string path = controller.DestroyChildAssets<AnimatorController, AnimationClip>();

            var layer = controller.layers[0];
            foreach (var state in layer.stateMachine.states) {
                layer.stateMachine.RemoveState(state.state);
            }

            var states = new Dictionary<BedAnimation, AnimatorState>();

            foreach (var anim in data.meta.frameTags) {
                if (!Enum.TryParse<BedAnimation>(anim.name, out var animation)) {
                    Debug.LogError($"Unknown animation '{anim.name}'!");
                    continue;
                }

                if (!isLoopingOverride.TryGetValue(animation, out bool isLooping)) {
                    isLooping = true;
                }

                var animClip = new AnimationClip() {
                    name = GetAnimationName(animation),
                    wrapMode = anim.direction switch {
                        AsepriteDataFrameDirection.forward => isLooping ? WrapMode.Loop : WrapMode.ClampForever,
                        AsepriteDataFrameDirection.pingpong => WrapMode.PingPong,
                        _ => throw new NotImplementedException(anim.direction.ToString()),
                    },
                };

                var settings = AnimationUtility.GetAnimationClipSettings(animClip);
                settings.loopTime = isLooping;
                AnimationUtility.SetAnimationClipSettings(animClip, settings);

                var keyframes = new List<ObjectReferenceKeyframe>();
                int time = 0;
                for (int i = anim.from; i <= anim.to; i++) {
                    keyframes.Add(new() {
                        time = time * TIME_MULTIPLIER,
                        value = GetSprite(i)
                    });
                    time += data.frames[i].duration;
                }
                keyframes.Add(new() {
                    time = time * TIME_MULTIPLIER,
                    value = GetSprite(isLooping ? anim.from : anim.to)
                });

                AnimationUtility.SetObjectReferenceCurve(
                    animClip,
                    EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"),
                    keyframes.ToArray()
                );

                AssetDatabase.AddObjectToAsset(animClip, path);

                states[animation] = controller.AddMotion(animClip);
            }

            void addTransition(BedAnimation from, BedAnimation to) {
                var transition = states[from].AddTransition(states[to]);
                transition.hasExitTime = true;
                transition.exitTime = 1;
                transition.duration = 0;
            }

            addTransition(BedAnimation.GoToSleep, BedAnimation.Sleep);
            addTransition(BedAnimation.DreamUp, BedAnimation.Dreaming);
            addTransition(BedAnimation.WakeUp, BedAnimation.Awake);

            if (animatorPrefab) {
                animatorPrefab.runtimeAnimatorController = controller;
                EditorUtility.SetDirty(animatorPrefab);
            }

            AssetDatabase.SaveAssets();
        }

        [ContextMenu(nameof(LoadAll))]
        void LoadAll() {
            LoadPrefab();
            LoadData();
            LoadSprites();
            LoadController();
        }
#endif
        public static string GetAnimationName(BedAnimation animation) {
            return animation.ToString();
        }
    }
}
