using Camera;
using Gameplay.Map;
using UnityEngine;

namespace UI.Util
{
    public class ScreenDistortion : MonoBehaviour
    {
        private Scriptable.Map loadedAtMap;
        
        
        
        private void OnDestroy()
        {
            MapManager.OnAfterMapLoad -= AfterMapLoad;
        }

        private void AfterMapLoad(Scriptable.Map map)
        {
            if (loadedAtMap.Equals(map)) return;
            
            Destroy(gameObject);
        }

        private void Awake()
        {
            transform.SetParent(MainCamera.Camera.transform);
            transform.localPosition = Vector3.zero;
            
            Update();
            loadedAtMap = MapManager.Map;
            MapManager.OnAfterMapLoad += AfterMapLoad;
        }

        private void Update()
        {
            var cam = MainCamera.Camera;

            float sizeY = cam.orthographicSize * 2;
            float sizeX = sizeY * cam.aspect;
            transform.localScale = new Vector3(sizeX, sizeY, 1);
        }
    }
}