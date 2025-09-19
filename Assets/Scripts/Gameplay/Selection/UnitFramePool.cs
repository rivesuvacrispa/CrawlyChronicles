using UnityEngine;

namespace Gameplay.Selection
{
    public class UnitFramePool : MonoBehaviour
    {
        public static UnitFramePool Instance { get; private set; }
        
        [SerializeField] private SelectableOverlay overlayPrefab;



        private UnitFramePool() => Instance = this;

        public SelectableOverlay CreateFrame(SelectableObject selectableObject)
        {
            var overlay = Instantiate(overlayPrefab, transform);
            overlay.SelectableObject = selectableObject;
            return overlay;
        }
    }
}