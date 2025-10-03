using Camera;
using UnityEngine;
using UnityEngine.UI;
using Util.Interfaces;

namespace Gameplay.Selection
{
    public class SelectableOverlay : MonoBehaviour
    {
        [SerializeField] private Image frameImage;
        
        private SelectableObject selectableObject;
        
        public SelectableObject SelectableObject
        {
            set
            {
                selectableObject = value;
                selectableObject.OnProviderDestroy += OnProviderDestroy;
            }
        }

        public void SetColor(Color c) => frameImage.color = c;
        
        public void SetFrameActive(bool isActive)
        {
            if(isActive) UpdatePosition();
            enabled = isActive;
            frameImage.enabled = isActive;
        }

        private void Update() => UpdatePosition();

        private void UpdatePosition() => transform.localPosition = UICamera.Camera.WorldToScreenPoint(selectableObject.Transform.position);

        private void OnProviderDestroy(IDestructionEventProvider target)
        {
            target.OnProviderDestroy -= OnProviderDestroy;
            Destroy(gameObject);
        }
    }
}