using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Selection
{
    public class SelectionFrame : MonoBehaviour
    {
        [SerializeField] private new BoxCollider2D collider;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;
        [SerializeField] private Color blueColor;
        [SerializeField] private Color redColor;
        
        public List<SelectableObject> selectedObjects { get; } = new();
        private SelectionMode selectionMode = SelectionMode.Overwrite;
        
        public void UpdateFrame(Vector2 sizeDelta, Vector2 scale)
        {
            collider.offset = new Vector2(sizeDelta.x / 2f, sizeDelta.y / -2f);
            collider.size = sizeDelta;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.localScale = scale;
        }
        
        public void SetActive(bool isActive, SelectionMode mode = SelectionMode.NotInitialized)
        {
            if (mode == SelectionMode.NotInitialized) mode = selectionMode;
            selectionMode = mode;
            image.color = selectionMode == SelectionMode.Remove ? redColor : blueColor;
            gameObject.SetActive(isActive);
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!col.gameObject.TryGetComponent(out SelectionCollider c)) return;
            
            SelectableObject o = c.SelectableObject;
            bool remove = selectionMode == SelectionMode.Remove;
            if((remove && !o.Selected) ||
               (selectionMode == SelectionMode.Add && o.Selected)) return;
            if(selectedObjects.Contains(o)) return;

            selectedObjects.Add(o);
            if(remove) o.Overlay.SetColor(redColor);
            else
            {
                o.Overlay.SetColor(blueColor);
                o.SetSelected(true);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if(!col.gameObject.TryGetComponent(out SelectionCollider c)) return;
            
            SelectableObject o = c.SelectableObject;
            bool remove = selectionMode == SelectionMode.Remove;
            if(remove && !o.Selected) return;
            if(!selectedObjects.Contains(o)) return;

            selectedObjects.Remove(o);
            if(remove) o.Overlay.SetColor(blueColor);
            else o.SetSelected(false);
        }

        private void OnEnable() => selectedObjects.Clear();
        private void OnDisable() => selectedObjects.Clear();
    }
}