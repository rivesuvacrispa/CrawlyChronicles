using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private FollowMovement followMovement;
        [SerializeField] private FreeMovement freeMovement;

        private UnityEngine.Camera cam;
        private static MainCamera instance;
        
        public static Vector2 WorldMousePos { get; private set; }
        public static FollowMovement FollowMovement => instance.followMovement;


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
            instance = this;
            cam = GetComponent<UnityEngine.Camera>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
                ToggleFollowMode();
            else if(Input.GetKeyDown(KeyCode.PageUp))
                ToggleFreeMode();
            WorldMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}