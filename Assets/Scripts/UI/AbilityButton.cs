using System.Collections;
using Definitions;
using Gameplay.Abilities;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class AbilityButton : BasicAbilityButton
    {
        [SerializeField] private Text hotkeyText;
        [SerializeField] private Image cooldownImage;

        private bool onCooldown;

        public override void SetAbility(BasicAbility newAbility)
        {
            base.SetAbility(newAbility);
            var activeAbility = (ActiveAbility) newAbility;
            cooldownImage.color = GlobalDefinitions
                .GetGeneColor(activeAbility.Scriptable.GeneType)
                .WithAlpha(0.35f);
            cooldownImage.fillAmount = 0;
            hotkeyText.text = activeAbility.Scriptable.KeyCode.ToString();
        }

        private void Update()
        {
            if(onCooldown) return;
            if(Input.GetKeyDown(((ActiveMutation)Scriptable).KeyCode)) 
                Activate();
        }

        public void Activate()
        {
            if(onCooldown) return;

            ActiveAbility activeAbility = (ActiveAbility) ability;
            activeAbility.Activate();
            StartCoroutine(CooldownRoutine(activeAbility.Cooldown));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            onCooldown = true;
            float t = duration;
            while (t > 0)
            {
                hotkeyText.text = Mathf.CeilToInt(t).ToString();
                cooldownImage.fillAmount = t / duration;
                t -= Time.deltaTime;
                yield return null;
            }

            hotkeyText.text = ((ActiveMutation)Scriptable).KeyCode.ToString();
            cooldownImage.fillAmount = 0;
            onCooldown = false;
        }

        private void OnDisable()
        {
            if(Scriptable is null) return;
            hotkeyText.text = ((ActiveMutation)Scriptable).KeyCode.ToString();
            cooldownImage.fillAmount = 0;
            onCooldown = false;
        }
    }
}