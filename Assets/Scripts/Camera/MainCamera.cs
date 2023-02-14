using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MainCamera : MonoBehaviour
    {
        private UnityEngine.Camera cam;
        
        public static Vector3 WorldMousePos { get; private set; }

        
        
        private void Awake() => cam = GetComponent<UnityEngine.Camera>();

        private void Update() => WorldMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}