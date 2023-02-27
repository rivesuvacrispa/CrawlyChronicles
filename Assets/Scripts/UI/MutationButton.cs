using System;
using Definitions;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class MutationButton : MonoBehaviour, IAbilityTooltipProvider
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image icon;
        [SerializeField] private Text lvlText;
        [SerializeField] private Button button;

        public BasicMutation Scriptable { get; private set; }
        public Action<BasicMutation, int> OnClick { get; set; }
        public int Lvl { get; private set; }
        
        public void SetMutation(BasicMutation mutation, int lvl)
        {
            Lvl = lvl;
            Scriptable = mutation;
            bgImage.color = GlobalDefinitions.GetGeneColor(mutation.GeneType).WithAlpha(0.5f);
            icon.sprite = mutation.Sprite;
            icon.color = mutation.SpriteColor;
            lvlText.text = GlobalDefinitions.GetRomanDigit(Lvl);
        }

        public void SetAffordable(bool isAffordable)
        {
            bgImage.color = bgImage.color.WithAlpha(isAffordable ? 0.5f : 0.2f);
            // icon.color = icon.color.WithAlpha(isAffordable ? 1f : 0.8f);
            lvlText.color = lvlText.color.WithAlpha(isAffordable ? 1f : 0.75f);
            button.interactable = isAffordable;
        }

        public void OnPopupEnd()
        {
            
        }
        
        public void Click()
        {
            OnClick(Scriptable, Lvl);
            gameObject.SetActive(false);
        }


        // IAbilityTooltipProvider
        public BasicMutation TooltipData => Scriptable;
        public int Level => Lvl;
        public bool ShowUpgradeStats => true;
    }
}