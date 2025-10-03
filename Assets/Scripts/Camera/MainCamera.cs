using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private FollowMovement followMovement;
        [SerializeField] private FreeMovement freeMovement;
        [SerializeField] private new UnityEngine.Camera camera;

        public static UnityEngine.Camera Camera => instance.camera;
        private static MainCamera instance;
        
        public static Vector2 WorldMousePos { get; private set; }
        public static FollowMovement FollowMovement => instance.followMovement;

        
        
        private MainCamera() => instance = this;

        public static void ToggleFreeMode()
        {
            instance.followMovement.enabled = false;
            instance.freeMovement.enabled = true;
        }

        public static void ToggleFollowMode()
        {
            instance.followMovement.enabled = true;
            instance.freeMovement.enabled = false;
        }
        
        private void Awake()
        {
            freeMovement.enabled = false;
            camera = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
                ToggleFollowMode();
            else if(Input.GetKeyDown(KeyCode.PageUp))
                ToggleFreeMode();
            WorldMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}