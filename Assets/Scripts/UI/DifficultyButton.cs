using Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI
{
    public class DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private LocalizedString localizedNameString;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private DifficultySelectionFrame selectionFrame;
        [SerializeField, Range(-1, 1)] private int position;
        [SerializeField] private Animator animator;
        [SerializeField] private Difficulty difficulty;

        
        private int idleAnimHash;
        private int animHash;
        private bool isSelected;
        private string description;

        private static DifficultyButton selectedButton;

        public static Difficulty SelectedDifficulty => selectedButton.difficulty;

        private void OnEnable()
        {
            localizedNameString.StringChanged += OnLocalizedNameStringChanged;
            if(isSelected)
            {
                PlaySelected();
                UpdateDescriptionText();
            }
        }

        private void OnDisable() => localizedNameString.StringChanged -= OnLocalizedNameStringChanged;

        private void OnLocalizedNameStringChanged(string text)
        {
            nameText.text = text;
            description = difficulty.GetDescription();
            if(isSelected) UpdateDescriptionText();
        }

        public void Select()
        {
            if(selectedButton is not null)
                selectedButton.isSelected = false;
            isSelected = true;
            selectedButton = this;
            selectionFrame.SetTargetX(position);
            
            if(!gameObject.activeInHierarchy) return;
            UpdateDescriptionText();
            PlaySelected();
        }

        private void UpdateDescriptionText() => descriptionText.text = description;

        private void Awake()
        {
            idleAnimHash = Animator.StringToHash($"{difficulty.OverallDifficulty}Idle");
            animHash = Animator.StringToHash(difficulty.OverallDifficulty.ToString());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(isSelected) return;
            
            UpdateDescriptionText();
            selectedButton.PlayIdle();
            selectionFrame.SetTargetX(position);
            PlaySelected();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(isSelected) return;

            PlayIdle();
            selectionFrame.SetTargetX(selectedButton.position);
            selectedButton.PlaySelected();
            selectedButton.UpdateDescriptionText();
        }

        private void PlaySelected() => animator.Play(animHash);
        private void PlayIdle() => animator.Play(idleAnimHash);
    }
}