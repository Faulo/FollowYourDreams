using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using FollowYourDreams.Avatar;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FollowYourDreams {
    static class EditorExtensions {
#if UNITY_EDITOR
        public static Sprite[] LoadSprites(this Texture2D sheet) {
            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(sheet))
                .OfType<Sprite>()
                .OrderBy(sprite => int.Parse(Regex.Match(sprite.name, @"\d+").Value))
                .ToArray();
        }
        public static Sprite ExtractSprite(this Texture2D sheet, AsepriteData data, Vector2 pivot, UnityObject target) {
            var sprites = new List<Sprite>();
            sheet.ExtractSprites(data, pivot, target, sprites, 1);
            return sprites.Count > 0
                ? sprites[0]
                : default;
        }
        public static void ExtractSprites(this Texture2D sheet, AsepriteData data, Vector2 pivot, UnityObject target, List<Sprite> sprites, int columns = 1) {
            string path = target.DestroyChildAssets<UnityObject, Sprite>();

            sprites.Clear();

            foreach (var frame in data.frames) {
                var rect = frame.frame.AsRectInt();
                int width = rect.width / columns;
                for (int i = 0; i < columns; i++) {
                    var spriteRect = new Rect(
                        rect.x + (width * i),
                        data.meta.size.h - rect.y - rect.height,
                        width,
                        rect.height
                    );
                    var sprite = Sprite.Create(
                        sheet,
                        spriteRect,
                        pivot,
                        GameManager.pixelsPerUnit,
                        0,
                        SpriteMeshType.FullRect,
                        default,
                        false,
                        Array.Empty<SecondarySpriteTexture>()
                    );

                    sprite.name = columns == 1
                        ? frame.filename
                        : frame.filename + "-" + i;

                    AssetDatabase.AddObjectToAsset(sprite, path);

                    sprites.Add(sprite);
                }
            }

            AssetDatabase.SaveAssets();
        }
        public static string DestroyChildAssets<TObject, TAsset>(this TObject obj)
            where TObject : UnityObject
            where TAsset : UnityObject {

            string path = AssetDatabase.GetAssetPath(obj);

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path).OfType<TAsset>()) {
                AssetDatabase.RemoveObjectFromAsset(asset);
            }

            return path;
        }
        public static Rect AsRect(this in RectInt rect) => new(rect.x, rect.y, rect.width, rect.height);

        const float TIME_MULTIPLIER = 0.001f;
        public static Action<TAnim, TAnim> ImportAnimations<TAnim>(
            this AnimatorController controller, AsepriteData data, IReadOnlyDictionary<TAnim, bool> isLoopingOverride, IReadOnlyList<Sprite> sprites, Func<TAnim, int, string> namer, int columns = 1) where TAnim : struct {
            Sprite getSprite(int index, int column) {
                return sprites[(index * columns) + column];
            }

            string path = controller.DestroyChildAssets<AnimatorController, AnimationClip>();

            var layer = controller.layers[0];
            foreach (var state in layer.stateMachine.states) {
                layer.stateMachine.RemoveState(state.state);
            }

            var states = new Dictionary<TAnim, AnimatorState>();

            for (int c = 0; c < columns; c++) {
                foreach (var anim in data.meta.frameTags) {
                    if (!Enum.TryParse<TAnim>(anim.name, out var animation)) {
                        Debug.LogError($"Unknown animation '{anim.name}'!");
                        continue;
                    }

                    if (!isLoopingOverride.TryGetValue(animation, out bool isLooping)) {
                        isLooping = true;
                    }

                    var animClip = new AnimationClip() {
                        name = namer(animation, c),
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
                            value = getSprite(i, c)
                        });
                        time += data.frames[i].duration;
                    }
                    keyframes.Add(new() {
                        time = time * TIME_MULTIPLIER,
                        value = getSprite(isLooping ? anim.from : anim.to, c)
                    });

                    AnimationUtility.SetObjectReferenceCurve(
                        animClip,
                        EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"),
                        keyframes.ToArray()
                    );

                    AssetDatabase.AddObjectToAsset(animClip, path);

                    states[animation] = controller.AddMotion(animClip);
                }
            }

            return (TAnim from, TAnim to) => {
                var transition = states[from].AddTransition(states[to]);
                transition.hasExitTime = true;
                transition.exitTime = 1;
                transition.duration = 0;
            };
        }
#endif
    }
}
