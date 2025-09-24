using Gameplay.Map;
using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class FreeMovement : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera uiCam;
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        [SerializeField] private float zoomStep;


        private UnityEngine.Camera cam;
        private Vector3 dragOrigin;
    
        private void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        
            if (Input.GetMouseButtonDown(2))
                dragOrigin = mouseWorldPos;

            if (Input.GetMouseButton(2))
            {
                Vector3 diff = dragOrigin - mouseWorldPos;
                Vector3 camPos = transform.position + diff;

                Vector3 minPos = MapManager.MinPoint.position;
                Vector3 maxPos = MapManager.MaxPoint.position;

                camPos = new Vector3(
                    Mathf.Clamp( camPos.x, minPos.x, maxPos.x), 
                    Mathf.Clamp( camPos.y, minPos.y, maxPos.y), 
                    camPos.z);
            
                transform.position = camPos;
            }

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0f)
            {
                float size = Mathf.Clamp(cam.orthographicSize + wheel * zoomStep * -1, minZoom, maxZoom);
                cam.orthographicSize = size;
                uiCam.orthographicSize = size;
            }
        }
    }
}