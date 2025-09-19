using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts.Gameplay.Map
{
    public class LocationMap : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap diggableTilemap;
        [SerializeField] private Tilemap mapwallsTilemap;

        private DiggableWall[,] walls;
        private Vector3Int size;
        private Vector3Int min;

        public Grid Grid => grid;
        public Tilemap Tilemap => diggableTilemap;
        
        private void Start()
        {
            min = diggableTilemap.cellBounds.min;
            size = diggableTilemap.size;
            walls = new DiggableWall[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(x + min.x, y + min.y, 0);
                if (diggableTilemap.GetTile(pos) == null ||
                    mapwallsTilemap.GetTile(pos) != null) continue;
                walls[x, y] = new DiggableWall();
            }
        }

        public bool GetDiggableWall(Vector3Int tilePos, out DiggableWall wall)
        {
            wall = null;
            tilePos -= min;
            if (tilePos.x < 0 || tilePos.y < 0 || tilePos.x >= size.x || tilePos.y >= size.y) return false;

            wall = walls[tilePos.x, tilePos.y];
            return wall is not null;
        }
    }
}