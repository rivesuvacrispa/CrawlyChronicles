using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InteractionPopup : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Text buttonText;
        [SerializeField] private Text actionText;
        [SerializeField] private Image fillingImage;

        private bool isActive;
        
        private static readonly int PopoutHash = Animator.StringToHash("InteractionPopout");
        private static readonly int PopupHash = Animator.StringToHash("InteractionPopup");

        public void SetFilling(float value) => fillingImage.fillAmount = value;
        
        public void Enable(string action, KeyCode key)
        {
            if(isActive) return;
            buttonText.text = key.ToString();
            actionText.text = action;
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