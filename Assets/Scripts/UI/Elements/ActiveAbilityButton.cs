using System;
using Controls;
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

        public delegate void KeyCodeEvent(KeyCode keyCode);
        public static event KeyCodeEvent OnKeyReserved;
        public static event KeyCodeEvent OnKeyUnreserved;
        
        private KeyCode? reservedKey;
        
        

        public override void SetAbility(ActiveAbility newAbility)
        {
            base.SetAbility(newAbility);

            cooldownImage.color = GlobalDefinitions
                .GetGeneColor(Ability.Scriptable.GeneType)
                .WithAlpha(0.35f);
            cooldownImage.fillAmount = 0;
            
            SetTextToHotkey();
            Ability.OnCooldownChanged += UpdateCooldown;
        }

        private void Update()
        {
            if (reservedKey is not null && Input.GetKeyDown(reservedKey.Value))
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
                SetTextToHotkey();
                cooldownImage.fillAmount = 0;
            }
        }

        private void OnEnable()
        {
            transform.SetAsLastSibling();
            if (Ability is null) return;

            ReserveKey();
            SetTextToHotkey();
            
            if (Ability.Autocast)
                autocastParticles.Play();
            else
                autocastParticles.Stop();
        }

        private void OnDisable()
        {
            if (Scriptable is null) return;

            UnreserveKey();
            SetTextToHotkey();
            cooldownImage.fillAmount = 0;
        }

        private void SetTextToHotkey()
        {
            string keyText = reservedKey?.ToString() ?? string.Empty;
            keyText = keyText.Replace("Alpha", "");
            hotkeyText.text = keyText;
        }
        
        private void ReserveKey()
        {
            UnreserveKey();
            
            if (ControlsManager.TryGetFreeKeyCode(out KeyCode keyCode))
            {
                reservedKey = keyCode;
                OnKeyReserved?.Invoke(keyCode);
            }
        }

        private void UnreserveKey()
        {
            if (reservedKey is not null)
            {
                OnKeyUnreserved?.Invoke(reservedKey.Value);
                reservedKey = null;
            }
        }
    }
}