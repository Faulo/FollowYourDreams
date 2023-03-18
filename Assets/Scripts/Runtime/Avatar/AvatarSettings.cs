using System;
using System.Collections.Generic;
using System.Linq;
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
    sealed class AvatarSettings : ScriptableAsset, IImportable {
        [Header("Setup")]
        [SerializeField]
        GameObject prefab;
        [SerializeField]
        GameManager manager;

        [Header("Movement")]
        [SerializeField, Expandable]
        AvatarMovement real;
        [SerializeField, Expandable]
        AvatarMovement dream;
        [SerializeField, Expandable]
        AvatarMovement nightmare;

        public AvatarMovement movement => manager.currentDimension switch {
            Dimension.RealWorld => real,
            Dimension.Dreamscape => dream,
            Dimension.NightmareRealm => nightmare,
            _ => throw new NotImplementedException(),
        };

        readonly Dictionary<Power, bool> powers = Enum.GetValues(typeof(Power))
            .Cast<Power>()
            .ToDictionary(p => p, p => false);

        public bool HasPower(Power power) => powers[power];
        void TogglePower(Power power) => powers[power] = !powers[power];
        public void ToggleClimb() => powers[Power.Climb] = true;
        public void ToggleGlide() => powers[Power.Glide] = true;
        public void ToggleHighJump() => powers[Power.HighJump] = true;

#if UNITY_EDITOR
        const int DIRECTION_COUNT = 5;
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
        SerializableKeyValuePairs<AvatarAnimation, bool> isLoopingOverride = new();

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
            sheet.ExtractSprites(data, pivot, this, sprites, DIRECTION_COUNT);

            if (rendererPrefab) {
                rendererPrefab.sprite = GetSprite(0, AvatarDirection.Down);
                EditorUtility.SetDirty(rendererPrefab);
            }

            AssetDatabase.SaveAssets();
        }

        [ContextMenu(nameof(LoadController))]
        public void LoadController() {
            var addTransition = controller.ImportAnimations(data, isLoopingOverride, sprites, GetAnimationName, (AvatarAnimation.Idle, (int)AvatarDirection.Down), DIRECTION_COUNT);

            addTransition(AvatarAnimation.Land, AvatarAnimation.Idle);

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

        public static string GetAnimationName(AvatarDirection direction, AvatarAnimation animation) {
            return $"{direction}_{animation}";
        }
        static string GetAnimationName(AvatarAnimation animation, int direction) {
            return GetAnimationName((AvatarDirection)direction, animation);
        }
    }
}
