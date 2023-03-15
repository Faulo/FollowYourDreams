using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class BedSettings : ScriptableAsset {
        [SerializeField]
        GameObject prefab;

#if UNITY_EDITOR
        const int DIRECTION_COUNT = 5;
        const float TIME_MULTIPLIER = 0.001f;
        [Header("Animations")]
        [SerializeField]
        AnimatorController controller;
        [SerializeField]
        Texture2D sheet;
        [SerializeField]
        Vector2 pivot = new(0.5f, 0.5f);
        [SerializeField]
        List<Sprite> sprites = new();

        Sprite GetSprite(int index) {
            return sprites[index];
        }

        [SerializeField]
        TextAsset json;
        [SerializeField]
        AsepriteData data = new();
        [SerializeField]
        Animator animatorPrefab;

        [ContextMenu(nameof(LoadPrefab))]
        void LoadPrefab() {
            prefab.TryGetComponent(out animatorPrefab);
        }

        [ContextMenu(nameof(LoadData))]
        void LoadData() {
            data = AsepriteData.FromJson(json.text);
        }

        [ContextMenu(nameof(LoadSprites))]
        void LoadSprites() {
            sheet.ExtractSprites(data, pivot, this, sprites);

            AssetDatabase.SaveAssets();
        }

        [ContextMenu(nameof(LoadController))]
        void LoadController() {
            string path = controller.DestroyChildAssets<AnimatorController, AnimationClip>();

            var layer = controller.layers[0];
            foreach (var state in layer.stateMachine.states) {
                layer.stateMachine.RemoveState(state.state);
            }

            foreach (var anim in data.meta.frameTags) {
                if (!Enum.TryParse<BedAnimation>(anim.name, out var animation)) {
                    Debug.LogError($"Unknown animation '{anim.name}'!");
                    continue;
                }

                var animClip = new AnimationClip() {
                    name = GetAnimationName(animation),
                    wrapMode = anim.direction switch {
                        AsepriteDataFrameDirection.forward => WrapMode.Loop,
                        AsepriteDataFrameDirection.pingpong => WrapMode.PingPong,
                        AsepriteDataFrameDirection.reverse => WrapMode.ClampForever,
                        _ => throw new NotImplementedException(anim.direction.ToString()),
                    },
                };

                var settings = AnimationUtility.GetAnimationClipSettings(animClip);
                settings.loopTime = true;
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
                    value = GetSprite(anim.from)
                });

                AnimationUtility.SetObjectReferenceCurve(
                    animClip,
                    EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"),
                    keyframes.ToArray()
                );

                AssetDatabase.AddObjectToAsset(animClip, path);

                controller.AddMotion(animClip);
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
