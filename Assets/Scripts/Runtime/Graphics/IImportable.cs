using UnityEngine;

namespace FollowYourDreams.Graphics {
    interface IImportable {
#if UNITY_EDITOR
        Vector2 spritePivot { get; set; }
        void LoadAll();
#endif
    }
}
