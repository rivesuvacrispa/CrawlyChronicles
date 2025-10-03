using Gameplay.Map;
using Scriptable;
using UnityEngine;

namespace Camera
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField] private new UnityEngine.Camera camera;

        private void OnEnable()
        {
            MapManager.OnAfterMapLoad += OnAfterMapLoad;
        }

        private void OnDisable()
        {
            MapManager.OnAfterMapLoad -= OnAfterMapLoad;
        }

        private void Start()
        {
            camera.orthographicSize = MapManager.MinimapScale;
        }

        private void OnAfterMapLoad(Map _)
        {
            camera.orthographicSize = MapManager.MinimapScale;
        }
    }
}