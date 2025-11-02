using Definitions;
using Gameplay.Mutations;
using Scriptable;
using TMPro;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class AbstractAbilityButton<T, TJ> : MonoBehaviour, IAbilityTooltipProvider, IAbilityButton
    where T : BasicAbility 
    where TJ : BasicMutation
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected TMP_Text levelText;

        private int level;
        protected TJ Scriptable { get; private set; }
        protected T Ability { get; private set; }

        
        
        public virtual void SetAbility(T newAbility)
        {
            Ability = newAbility;
            Scriptable = (TJ) newAbility.Scriptable;
            SetVisuals(Scriptable);
            newAbility.Button = this;
        }

        public void SetVisuals(TJ mutation)
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