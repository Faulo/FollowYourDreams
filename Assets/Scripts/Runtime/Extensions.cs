using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FollowYourDreams.Avatar;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FollowYourDreams {
    static class Extensions {
#if UNITY_EDITOR
        public static Sprite[] LoadSprites(this Texture2D sheet) {
            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(sheet))
                .OfType<Sprite>()
                .OrderBy(sprite => int.Parse(Regex.Match(sprite.name, @"\d+").Value))
                .ToArray();
        }
        public static void ExtractSprites(this Texture2D sheet, AsepriteData data, Vector2 pivot, UnityObject target, List<Sprite> sprites) {
            string path = target.DestroyChildAssets<UnityObject, Sprite>();

            sprites.Clear();

            foreach (var frame in data.frames) {
                var sprite = Sprite.Create(
                    sheet,
                    frame.frame.AsRect(),
                    pivot,
                    GameManager.pixelsPerUnit,
                    0,
                    SpriteMeshType.FullRect,
                    default,
                    false,
                    Array.Empty<SecondarySpriteTexture>()
                );

                sprite.name = frame.filename;

                AssetDatabase.AddObjectToAsset(sprite, path);

                sprites.Add(sprite);
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
#endif
    }
}
