using System.Collections;
using Definitions;
using Gameplay.Abilities;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text hotkeyText;
        [SerializeField] private Image cooldownImage;

        private bool onCooldown;
        private ActiveMutation scriptable;
        private Ability ability;

        public void SetAbility(Ability newAbility)
        {
            ability = newAbility;
            scriptable = ability.Scriptable;
            cooldownImage.color = GlobalDefinitions
                .GetGeneColor(ability.Scriptable.GeneType)
                .WithAlpha(0.35f);
            cooldownImage.fillAmount = 0;
            hotkeyText.text = scriptable.KeyCode.ToString();
            icon.color = scriptable.SpriteColor;
            icon.sprite = scriptable.Sprite;
        }

        private void Update()
        {
            if(Input.GetKeyDown(scriptable.KeyCode)) Activate();
        }

        public void Activate()
        {
            if(onCooldown) return;
            
            ability.Activate();
            StartCoroutine(CooldownRoutine(ability.Cooldown));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            enabled = false;
            onCooldown = true;
            float t = duration;
            while (t > 0)
            {
                hotkeyText.text = Mathf.CeilToInt(t).ToString();
                cooldownImage.fillAmount = t / duration;
                t -= Time.deltaTime;
                yield return null;
            }

            cooldownImage.fillAmount = 0;
            hotkeyText.text = scriptable.KeyCode.ToString();
            enabled = true;
            onCooldown = false;
        }
    }
}