using UnityEngine;

namespace Gameplay.Selection
{
    public class SelectionCollider : MonoBehaviour
    {
        [SerializeField] private SelectableObject selectableObject;

        public SelectableObject SelectableObject => selectableObject;
        
        /*
        private void OnTriggerEnter2D(Collider2D col)
        {
            switch (ObjectSelector.SelectionMode)
            {
                case SelectionMode.Add or SelectionMode.Overwrite:
                    selectableObject.SetSelected(true);
                    break;
                case SelectionMode.Remove:
                    selectableObject.SetSelected(false);
                    break;
            }
        }

        /*private void OnTriggerExit2D(Collider2D other) => selectableObject.SetSelected(false);#1#
        
        private void OnMouseDown()
        {
            Debug.Log("Click");
        }*/
    }
}