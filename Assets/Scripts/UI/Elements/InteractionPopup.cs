using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class InteractionPopup : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private Image fillingImage;

        private bool isActive;
        
        private static readonly int PopoutHash = Animator.StringToHash("InteractionPopout");
        private static readonly int PopupHash = Animator.StringToHash("InteractionPopup");

        public void SetFilling(float value) => fillingImage.fillAmount = value;
        
        public void Enable(string action, KeyCode key)
        {
            buttonText.text = key.ToString();
            actionText.text = action;
            if(isActive) return;
            gameObject.SetActive(true);
            animator.Play(PopupHash);
            isActive = true;
        }

        public void Disable()
        {
            if(!isActive) return;
            SetFilling(0);
            animator.Play(PopoutHash);
            isActive = false;
        }
        
    }
}