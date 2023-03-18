using System.Collections.Generic;
using FollowYourDreams.Graphics;
using MyBox;
using Slothsoft.UnityExtensions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;

namespace FollowYourDreams.Avatar {
    [CreateAssetMenu]
    sealed class BedSettings : ScriptableAsset, IImportable {
        [Header("Setup")]
        [SerializeField]
        GameObject prefab;

#if UNITY_EDITOR
        [Header("Editor-only")]
        [SerializeField]
        AnimatorController controller;
        [SerializeField]
        TextAsset json;
        [SerializeField]
        Texture2D sheet;
        [SerializeField]
        Vector2 pivot = new(0.5f, 0.5f);
        public Vector2 spritePivot {
            get => pivot;
            set => pivot = value;
        }
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
        public void LoadPrefab() {
            prefab.TryGetComponent(out animatorPrefab);
            prefab.TryGetComponent(out rendererPrefab);
        }

        [ContextMenu(nameof(LoadData))]
        public void LoadData() {
            data = AsepriteData.FromJson(json.text);
        }

        [ContextMenu(nameof(LoadSprites))]
        public void LoadSprites() {
            sheet.ExtractSprites(data, pivot, this, sprites);

            if (rendererPrefab) {
                rendererPrefab.sprite = GetSprite(0);
                EditorUtility.SetDirty(rendererPrefab);
            }

            AssetDatabase.SaveAssets();
        }

        [ContextMenu(nameof(LoadController))]
        public void LoadController() {
            var addTransition = controller.ImportAnimations(data, isLoopingOverride, sprites, GetAnimationName, (BedAnimation.BedEmpty, 0));

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
        public void LoadAll() {
            LoadPrefab();
            LoadData();
            LoadSprites();
            LoadController();
        }
#endif
        static string GetAnimationName(BedAnimation animation, int _) {
            return GetAnimationName(animation);
        }
        public static string GetAnimationName(BedAnimation animation) {
            return animation.ToString();
        }
    }
}
