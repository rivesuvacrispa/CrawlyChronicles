﻿using Gameplay.Player;
using Scriptable;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltips
{
    public class AbilityTooltip : MonoBehaviour
    {
        [SerializeField] protected GameObject rootGO;
        [SerializeField] protected MutationButton button;
        [SerializeField] protected Text nameText;
        [SerializeField] protected Text descriptionText;

        
        public virtual void SetTooltip(BasicMutation mutation, int lvl, bool withUpgrade)
        {
            button.SetMutation(mutation, lvl);
            nameText.text = mutation.Name;
            descriptionText.text = mutation.Description + "\n\n" + AbilityController
                .GetAbilityLevelDescription(mutation, lvl, withUpgrade);
            rootGO.SetActive(true);
        }
        
        public void Clear() => rootGO.SetActive(false);
    }
}