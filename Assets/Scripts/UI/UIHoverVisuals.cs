using SoundEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIHoverVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool addSound = true;
        
        private Vector3 initialScale;

        private void Awake()
        {
            initialScale = transform.localScale;
            if(!addSound) return;
            var btn = GetComponentInChildren<Button>();
            if (btn is not null) btn.onClick.AddListener(UIAudioController.Instance.PlaySelect);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = initialScale * 1.1f;
            UIAudioController.Instance.PlayHover();
        }

        private void OnDisable() => transform.localScale = initialScale;

        public void OnPointerExit(PointerEventData eventData) => transform.localScale = initialScale;
    }
}