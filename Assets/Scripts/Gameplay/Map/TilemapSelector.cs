using UnityEngine;

namespace Scripts.Gameplay.Map
{
    public class TilemapSelector : MonoBehaviour
    {
        [SerializeField] private LocationMap locationMap;
        [SerializeField] private UnityEngine.Camera cam;
        [SerializeField] private Transform selectionTransform;


        private void Update()
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            var tilePos = locationMap.Tilemap.WorldToCell(mousePos);
            tilePos.z = 0;

            bool hasWall = locationMap.GetDiggableWall(tilePos, out _);
            if (hasWall)
            {
                selectionTransform.position = locationMap.Grid.GetCellCenterWorld(tilePos);
                selectionTransform.gameObject.SetActive(true);
            } else selectionTransform.gameObject.SetActive(false);

        }
    }
}