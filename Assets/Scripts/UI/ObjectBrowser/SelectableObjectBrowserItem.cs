using Gameplay.Selection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ObjectBrowser
{
    public class SelectableObjectBrowserItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image frameImage;

        private SelectableObject selectableObject;
        
        public SelectableObject SelectableObject
        {
            set
            {
                selectableObject = value;
                selectableObject.OnUnselected += OnUnselected;
                iconImage.sprite = selectableObject.Selectable.Sprite;
                SetFrameSelected(true);
            }
        }

        
        
        public void SetFrameSelected(bool isSelected) => SetFrameColor(isSelected ? Color.white : Color.gray);
        
        private void SetFrameColor(Color c) => frameImage.color = c;
        
        private void OnUnselected(SelectableObject o)
        {
            o.OnUnselected -= OnUnselected;
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}