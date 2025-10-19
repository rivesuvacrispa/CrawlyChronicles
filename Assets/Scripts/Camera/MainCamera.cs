using DG.Tweening;
using UnityEngine;

namespace Camera
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private FollowMovement followMovement;
        [SerializeField] private FreeMovement freeMovement;
        [SerializeField] private new UnityEngine.Camera camera;

        public static UnityEngine.Camera Camera => Instance.camera;
        public static MainCamera Instance { get; private set; }
        
        public static Vector2 WorldMousePos { get; private set; }
        public static FollowMovement FollowMovement => Instance.followMovement;

        
        
        private MainCamera() => Instance = this;

        public static void ToggleFreeMode()
        {
            Instance.followMovement.enabled = false;
            Instance.freeMovement.enabled = true;
        }

        public static void ToggleFollowMode()
        {
            Instance.followMovement.enabled = true;
            Instance.freeMovement.enabled = false;
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

        public void Shake(float strength) => transform.DOShakePosition(0.5f, 0.075f * Mathf.Clamp01(strength), 30);
    }
}