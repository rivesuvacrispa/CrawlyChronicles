using Gameplay.Map;
using UnityEngine;

namespace Util
{
    public class AstarManager : MonoBehaviour
    {
        [SerializeField] private AstarPath astarPath;
        
        private void OnEnable()
        {
            MapManager.OnAfterMapLoad += OnAfterMapLoad;
        }

        private void OnDisable()
        {
            MapManager.OnAfterMapLoad -= OnAfterMapLoad;
        }

        private void OnAfterMapLoad()
        {
            astarPath.Scan();
            Debug.Log("AstarPath re-scanned");
        }
    }
}