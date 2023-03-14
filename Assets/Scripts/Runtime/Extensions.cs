using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FollowYourDreams {
    static class Extensions {
#if UNITY_EDITOR
        public static Sprite[] LoadSprites(this Texture2D sheet) {
            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(sheet))
                .OfType<Sprite>()
                .OrderBy(sprite => int.Parse(Regex.Match(sprite.name, @"\d+").Value))
                .ToArray();
        }
#endif
    }
}
