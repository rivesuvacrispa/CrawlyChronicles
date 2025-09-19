using Scripts.OpenWorld.Generation.Noise;
using Scripts.OpenWorld.ShadowCasters;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace Scripts.OpenWorld.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField, Range(20, 200)] private int worldSize;
        [SerializeField, Range(0, 1)] private float step;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase tileToPaint;
        [SerializeField] private TilemapShadowCaster2D shadowCaster2D;

        

        public void Clear() => tilemap.ClearAllTiles();
        
        public void Generate()
        {
            Clear();
            int seed = Random.Range(-10000, 10000);
            var noise = GetComponentInChildren<NoiseGenerator>().GenerateNoise(worldSize, seed);
            Paint(noise);
        }
        
        public void UpdateShadowCaster() => shadowCaster2D.UpdateShadow();
        
        private void Paint(float[,] noise)
        {
            for (int x = 0; x < worldSize; x++)
            for (int y = 0; y < worldSize; y++)
            {
                float value = noise[x, y];
                if(value >= step) tilemap.SetTile(new Vector3Int(y, x, 0), tileToPaint);
            }
        }

    }
}