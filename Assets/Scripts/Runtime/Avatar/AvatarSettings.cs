using System;
using System.Collections.Generic;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class AvatarSettings : ScriptableAsset {
        [Header("Setup")]
        [SerializeField]
        GameObject prefab;

        [Header("Movement")]
        [SerializeField, Range(0, 10)]
        public float rotationSmoothing = 0.1f;
        [SerializeField, Range(0, 10)]
        public float speedSmoothing = 0.1f;
        [SerializeField, Range(0, 10)]
        public float walkSpeed = 1;
        [SerializeField, Range(0, 10)]
        public float runSpeed = 2;
        [SerializeField, Range(0, 10)]
        public float jumpSpeed = 5;

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

        [Header("Auto-filled")]
        [SerializeField, ReadOnly]
        List<Sprite> sprites = new();
        Sprite GetSprite(int index, AvatarDirection direction) {
            return sprites[(index * DIRECTION_COUNT) + (int)direction];
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
            sheet.ExtractSprites(data, pivot, this, sprites, DIRECTION_COUNT);

            if (rendererPrefab) {
                rendererPrefab.sprite = GetSprite(0, AvatarDirection.Down);
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

            foreach (AvatarDirection direction in Enum.GetValues(typeof(AvatarDirection))) {
                foreach (var anim in data.meta.frameTags) {
                    if (!Enum.TryParse<AvatarAnimation>(anim.name, out var animation)) {
                        Debug.LogError($"Unknown animation '{anim.name}'!");
                        continue;
                    }

                    var animClip = new AnimationClip() {
                        name = GetAnimationName(direction, animation),
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
                            value = GetSprite(i, direction)
                        });
                        time += data.frames[i].duration;
                    }
                    keyframes.Add(new() {
                        time = time * TIME_MULTIPLIER,
                        value = GetSprite(anim.from, direction)
                    });

                    AnimationUtility.SetObjectReferenceCurve(
                        animClip,
                        EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"),
                        keyframes.ToArray()
                    );

                    AssetDatabase.AddObjectToAsset(animClip, path);

                    controller.AddMotion(animClip);
                }
            }

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

        public static string GetAnimationName(AvatarDirection direction, AvatarAnimation animation) {
            return $"{direction}_{animation}";
        }
    }
}
