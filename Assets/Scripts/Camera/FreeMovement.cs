using UnityEngine;

namespace Camera
{
    public class FreeMovement : MonoBehaviour
    {
        [SerializeField] private Transform minPoint;
        [SerializeField] private Transform maxPoint;
    
        private Vector3 dragOrigin;
        private Vector3 minPos;
        private Vector3 maxPos;
    
        private void Awake()
        {
            minPos = minPoint.transform.position;
            maxPos = maxPoint.transform.position;
        }

        void Update()
        {
            Vector3 mouseWorldPos = MainCamera.WorldMousePos;
        
            if (Input.GetMouseButtonDown(1))
                dragOrigin = mouseWorldPos;

            if (Input.GetMouseButton(1))
            {
                Vector3 diff = dragOrigin - mouseWorldPos;
                Vector3 camPos = transform.position + diff;

                camPos = new Vector3(
                    Mathf.Clamp( camPos.x, minPos.x, maxPos.x), 
                    Mathf.Clamp( camPos.y, minPos.y, maxPos.y), 
                    camPos.z);
            
                transform.position = camPos;
            } 
        }
    }
}