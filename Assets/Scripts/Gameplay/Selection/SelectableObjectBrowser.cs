using System.Collections.Generic;
using UI.ObjectBrowser;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Selection
{
    public class SelectableObjectBrowser : MonoBehaviour
    {
        [SerializeField] private Text amountText;
        [SerializeField] private Transform itemsTransform;
        [SerializeField] private SelectableObjectBrowserItem itemPrefab;

        private readonly List<SelectableObject> selectedObjects = new();


        public void UnselectAll()
        {
            foreach (SelectableObject o in selectedObjects.ToArray()) 
                o.SetSelected(false);
        }

        public void Select(SelectableObject o)
        {
            if(selectedObjects.Contains(o)) return;
            
            var item = Instantiate(itemPrefab, itemsTransform);
            item.SelectableObject = o;
            o.OnUnselected += Unselect;
            selectedObjects.Add(o);
            UpdateAmountText();
            o.SetSelected(true, item);
        }

        private void UpdateAmountText()
        {
            int selectedAmount = selectedObjects.Count;
            amountText.text = $"Selected {selectedAmount} object{(selectedAmount == 1 ? string.Empty : "s")}";
            amountText.enabled = selectedAmount > 0;
        }

        private void Unselect(SelectableObject o)
        {
            if(!selectedObjects.Contains(o)) return;

            selectedObjects.Remove(o);
            o.OnUnselected -= Unselect;
            UpdateAmountText();
            
        }
    }
}