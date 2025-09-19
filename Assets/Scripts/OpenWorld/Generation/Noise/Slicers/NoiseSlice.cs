using System;
using OpenWorld.Generation.Noise.Layers;
using UnityEngine;

namespace OpenWorld.Generation.Noise.Slicers
{
    [RequireComponent(typeof(NoiseLayer))]
    public abstract class NoiseSlice : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] protected float minBound;
        [SerializeField, Range(0, 1)] protected float maxBound;
        [SerializeField, Range(0, 1)] private float baseLine;

        public float BaseLine => baseLine;


        public abstract bool IsWithinBounds(float value);

        private void Awake() => UpdateBounds();
        private void OnValidate() => UpdateBounds();

        private void UpdateBounds()
        {
            if(Math.Abs(minBound - maxBound) < float.Epsilon) return;
            float tMin = Mathf.Min(minBound, maxBound);
            float tMax = Mathf.Max(minBound, maxBound);
            minBound = tMin;
            maxBound = tMax;
        }
    }
}