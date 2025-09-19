using Scripts.OpenWorld.Generation.Noise.Layers;
using UnityEngine;

namespace Scripts.OpenWorld.Generation.Noise
{
    public class NoiseGenerator : MonoBehaviour
    {
        public float[,] GenerateNoise(int size, int seed)
        {
            var layers = FindLayers();
            var layer = CreateEmptyLayer(size);
            
            foreach (NoiseLayer noiseLayer in layers) 
                noiseLayer.ApplyNoise(layer, seed);

            return layer;
        }

        private float[,] CreateEmptyLayer(int size)
        {
            float[,] layer = new float[size, size];
            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
                layer[x, y] = 0;
            return layer;
        }

        private NoiseLayer[] FindLayers() => GetComponentsInChildren<NoiseLayer>();
    }
}