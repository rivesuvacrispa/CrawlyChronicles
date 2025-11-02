using Definitions;
using Gameplay.Mutations;
using Gameplay.Player;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Elements
{
    public class ActiveAbilityButton : AbstractAbilityButton<ActiveAbility, ActiveMutation>
    {
        [SerializeField] private TMP_Text hotkeyText;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private ParticleSystem autocastParticles;


        public override void SetAbility(ActiveAbility newAbility)
        {
            base.SetAbility(newAbility);

            cooldownImage.color = GlobalDefinitions
                .GetGeneColor(Ability.Scriptable.GeneType)
                .WithAlpha(0.35f);
            cooldownImage.fillAmount = 0;
            hotkeyText.text = Ability.Scriptable.KeyCode.ToString();
            Ability.OnCooldownChanged += UpdateCooldown;
        }

        private void Update()
        {
            if (Input.GetKeyDown(Scriptable.KeyCode))
                Activate();
        }

        public void ToggleAutocast()
        {
#if UNITY_EDITOR
            // In god mode, autocast cannot be enabled due to zero abilities cooldown
            if (PlayerManager.Instance.GodMode) return;
#endif

            Ability.Autocast = !Ability.Autocast;
            if (Ability.Autocast)
                autocastParticles.Play();
            else
                autocastParticles.Stop();
        }


        public void Activate()
        {
            if (Ability.CanActivate())
                Ability.Activate();
        }


        private void UpdateCooldown(float currentCooldown, float baseCooldown)
        {
            if (currentCooldown > 0)
            {
                hotkeyText.text = currentCooldown.ToString("0.0");
                cooldownImage.fillAmount = currentCooldown / (baseCooldown);
            }
            else
            {
                hotkeyText.text = Scriptable.KeyCode.ToString();
                cooldownImage.fillAmount = 0;
            }
        }

        private void OnEnable()
        {
            if (Ability is null) return;

            if (Ability.Autocast)
                autocastParticles.Play();
            else
                autocastParticles.Stop();
        }

        private void OnDisable()
        {
            if (Scriptable is null) return;

            hotkeyText.text = Scriptable.KeyCode.ToString();
            cooldownImage.fillAmount = 0;
        }
    }
}