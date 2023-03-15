using System;
using MyBox;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FollowYourDreams {
    [ExecuteAlways]
    abstract class DimensionFloatController<T> : MonoBehaviour where T : UnityObject {
        [Header("Setup")]
        [SerializeField, Expandable]
        GameManager manager;

        [Header("Dimensions")]
        [SerializeField]
        T real;
        [SerializeField]
        T dream;
        [SerializeField]
        T nightmare;

        [Header("Config")]
        [SerializeField, Range(0, 10)]
        float dimensionSmoothing = 1;

        [Header("Runtime")]
        [SerializeField, ReadOnly]
        float realVelocity;
        [SerializeField, ReadOnly]
        float dreamVelocity;
        [SerializeField, ReadOnly]
        float nightmareVelocity;

        protected void Update() {
            if (manager) {
                if (Application.isPlaying) {
                    SetDimension(real, Mathf.SmoothDamp(GetDimension(real), manager.realStrength, ref realVelocity, dimensionSmoothing));
                    SetDimension(dream, Mathf.SmoothDamp(GetDimension(dream), manager.dreamStrength, ref dreamVelocity, dimensionSmoothing));
                    SetDimension(nightmare, Mathf.SmoothDamp(GetDimension(nightmare), manager.nightmareStrength, ref nightmareVelocity, dimensionSmoothing));
                } else {
                    SetDimension(real, manager.realStrength);
                    SetDimension(dream, manager.dreamStrength);
                    SetDimension(nightmare, manager.nightmareStrength);
                }
            }
        }

        protected abstract float GetDimension(T dimension);

        protected abstract void SetDimension(T dimension, float strength);
    }
}
