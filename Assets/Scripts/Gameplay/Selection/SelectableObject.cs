using UI.ObjectBrowser;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Selection
{
    public class SelectableObject : MonoBehaviour, ITransformProvider
    {
        [SerializeField] private Scriptable.SelectableObject selectableObject;

        private SelectableObjectBrowserItem BrowserItem { set; get; }
        
        public delegate void SelectionEvent(SelectableObject o);
        public event SelectionEvent OnUnselected;
        public SelectableOverlay Overlay { get; private set; }
        public Scriptable.SelectableObject Selectable => selectableObject;
        public bool Selected { get; private set; }

        

        protected virtual void Start()
        {
            Overlay = UnitFramePool.Instance.CreateFrame(this);
        }

        protected virtual void OnDestroy()
        {
            if(Selected) OnUnselected?.Invoke(this);
            OnProviderDestroy?.Invoke(this);
        }

        public void SetSelected(bool isSelected, SelectableObjectBrowserItem browserItem = null)
        {
            if (Selected == isSelected) return;
            Selected = isSelected;
            BrowserItem = browserItem;
            Overlay.SetFrameActive(isSelected);
            if(!Selected) OnUnselected?.Invoke(this);
        }

        
        
        // ITransformProvider
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => gameObject.transform;
    }
}