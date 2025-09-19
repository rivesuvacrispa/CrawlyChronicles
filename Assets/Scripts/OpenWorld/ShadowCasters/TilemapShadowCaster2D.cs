using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Scripts.OpenWorld.ShadowCasters
{
    public class TilemapShadowCaster2D : MonoBehaviour
    {
        [SerializeField] protected CompositeCollider2D tilemapCollider;
        [SerializeField] protected TilemapRenderer tilemapRenderer;
        [SerializeField] protected bool selfShadows = true;
 
        private void Reset()
        {
            tilemapCollider = GetComponent<CompositeCollider2D>();
            tilemapRenderer = GetComponent<TilemapRenderer>();
        }

        public void UpdateShadow()
        {
            ShadowCaster2DGenerator.GenerateTilemapShadowCasters(tilemapCollider, selfShadows, tilemapRenderer);
        }
    }
}