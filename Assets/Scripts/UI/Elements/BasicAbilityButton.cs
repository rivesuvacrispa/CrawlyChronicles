using Definitions;
using Gameplay.Mutations;
using Scriptable;
using TMPro;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class BasicAbilityButton : MonoBehaviour, IAbilityTooltipProvider
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected TMP_Text levelText;

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

        public void SetActive(bool isActive)
        {
#if UNITY_EDITOR
            // Suppress annoying errors on editor game window playmode end
            try
            {
                gameObject.SetActive(isActive);
            } catch {}
#else
            gameObject.SetActive(isActive);
#endif
        }


        // IAbilityTooltipProvider
        public BasicMutation TooltipData => Scriptable;
        public int Level => level;
        public bool ShowUpgradeStats => false;
    }
}