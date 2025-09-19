using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Selection
{
    public class ObjectSelector : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private SelectionFrame selectionFrame;
        [SerializeField] private SelectableObjectBrowser browser;
        [SerializeField] private Physics2DRaycaster raycaster;
        [SerializeField] private UnityEngine.Camera cam;
        [Header("Settings")]
        [SerializeField] private float dragTimeTreshold;
        [SerializeField] private float dragDistanceTreshold;

        private float thresholdTimer;
        private bool dragInitialized;
        private bool dragging;
        private Vector2 dragOrigin;
        private RectTransform rectTransform;
        private readonly List<RaycastResult> raycastResults = new(1);

        private SelectionMode SelectionMode { get; set; } = SelectionMode.Overwrite;
        
        private void Awake() => rectTransform = GetComponentInParent<Canvas>().transform as RectTransform;

        private void Update()
        {
            bool pressed = dragInitialized ? Input.GetMouseButtonUp(0) : Input.GetMouseButtonDown(0);

            
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    rectTransform, 
                    Input.mousePosition,
                    cam, out var mousePos);

            if (pressed)
            {
                if (dragInitialized) 
                    CancelSelection();
                else
                {
                    bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                    SelectionMode = ctrl ? SelectionMode.Add : alt ? SelectionMode.Remove : SelectionMode.Overwrite;
                    if (ClickOnObject()) return;
                    InitializeDrag(mousePos);
                }
            } else if (dragInitialized && !dragging)
            {
                thresholdTimer += Time.deltaTime;
                float dist = Vector2.Distance(dragOrigin, mousePos);
                if(dist >= dragDistanceTreshold || thresholdTimer >= dragTimeTreshold) 
                    StartSelection(mousePos);
            } 
            
            if(dragging) UpdateSelectionFrame(mousePos);
        }

        private bool ClickOnObject()
        {
            raycastResults.Clear();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            raycaster.Raycast(pointerEventData, raycastResults);

            if (raycastResults.Count == 0 || 
                !raycastResults[0].gameObject
                    .TryGetComponent(out SelectionCollider c)) return false;

            SelectableObject o = c.SelectableObject;
            switch (SelectionMode)
            {
                case SelectionMode.Add:
                    browser.Select(o);
                    break;
                case SelectionMode.Remove:
                    o.SetSelected(false);
                    break;
                case SelectionMode.Overwrite:
                    browser.UnselectAll();
                    browser.Select(o);
                    break;
            }

            return true;
        }

        private void StartSelection(Vector2 mousePos)
        {
            if(SelectionMode is SelectionMode.Overwrite)
                browser.UnselectAll();
            dragging = true;
            transform.localPosition = dragOrigin;
            UpdateSelectionFrame(mousePos);
            selectionFrame.SetActive(true, SelectionMode);
        }

        private void InitializeDrag(Vector2 mousePos)
        {
            dragInitialized = true;
            dragOrigin = mousePos;
            thresholdTimer = 0f;
        }
        
        private void CancelSelection()
        {
            var selected = selectionFrame.selectedObjects.ToArray();
            dragInitialized = false;
            dragging = false;
            selectionFrame.SetActive(false);
            
            if(SelectionMode is SelectionMode.Remove)
                foreach (SelectableObject o in selected) 
                    o.SetSelected(false);
            else 
                foreach (SelectableObject o in selected) 
                    browser.Select(o);
        }

        private void UpdateSelectionFrame(Vector2 mousePos)
        {
            Vector2 diff = dragOrigin - mousePos;
            float x = Math.Abs(diff.x);
            float y = Math.Abs(diff.y);
            Vector2 size = new Vector2(x, y);
            selectionFrame.UpdateFrame(size, new Vector3(diff.x > 0 ? -1 : 1, diff.y > 0 ? 1 : -1, 1));
        }
    }
}