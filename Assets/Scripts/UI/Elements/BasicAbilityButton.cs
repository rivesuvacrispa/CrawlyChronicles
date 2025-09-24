using Definitions;
using Gameplay.Mutations;
using Scriptable;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class BasicAbilityButton : MonoBehaviour, IAbilityTooltipProvider
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Text levelText;

        public BasicMutation Scriptable { get; private set; }
        private int level;
        protected BasicAbility ability;

        
        public virtual void SetAbility(BasicAbility newAbility)
        {
            ability = newAbility;
            Scriptable = ability.Scriptable;
            SetVisuals(Scriptable);
            newAbility.Button = this;
        }

        public void SetVisuals(BasicMutation mutation)
        {
            Scriptable = mutation;
            icon.color = mutation.SpriteColor;
            icon.sprite = mutation.Sprite;
        }

        public void UpdateLevelText(int lvl)
        {
            level = lvl;
            levelText.text = GlobalDefinitions.GetRomanDigit(lvl);
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
        
        
        
        // IAbilityTooltipProvider
        public BasicMutation TooltipData => Scriptable;
        public int Level => level;
        public bool ShowUpgradeStats => false;
    }
}