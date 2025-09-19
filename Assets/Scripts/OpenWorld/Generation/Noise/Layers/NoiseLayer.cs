using OpenWorld.Generation.Noise.Slicers;
using UnityEngine;

namespace OpenWorld.Generation.Noise.Layers
{
    public abstract class NoiseLayer : MonoBehaviour
    {
        [SerializeField, Range(0, 100)] private float strength = 1f;
        [SerializeField, Range(0.001f, 100)] private float scale = 1f;



        protected abstract float CombineNoise(float left, float right);

        public void ApplyNoise(float[,] noise, int seed)
        {
            if(!isActiveAndEnabled) return;
            
            bool hasSlicer = TryGetComponent(out NoiseSlice slicer);
            int size = noise.GetLength(0);
            Vector2 offset = CreateOffset(seed, size);

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float xCord = offset.x + (x + 0.5f) / scale;
                float yCord = offset.y + y / scale;
                float noiseValue = Mathf.PerlinNoise(xCord, yCord);
                if (hasSlicer && !slicer.IsWithinBounds(noiseValue)) noiseValue = slicer.BaseLine;

                noise[x, y] = Mathf.Clamp01(CombineNoise(noise[x, y], noiseValue * strength));
            }
        }

        private static Vector2 CreateOffset(int seed, int size)
        {
            System.Random rnd = new System.Random(seed);
            int range = size * 10000;
            return new Vector2(rnd.Next(-range, range), rnd.Next(-range, range));
        }

        public static float[,] CreateNoise(Vector2 offset, int scale, int size)
        {
            float[,] noise = new float[size, size];
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float xCord = offset.x + (x + 0.5f) / scale;
                float yCord = offset.y + (float) y / scale;
                noise[x, y] = Mathf.Clamp01(Mathf.PerlinNoise(xCord, yCord));
            }

            return noise;
        }
    }
}