using System.Collections;
using Definitions;
using Gameplay.Abilities;
using Player;
using Scriptable;
using Timeline;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class ActiveAbilityButton : BasicAbilityButton
    {
        [SerializeField] private Text hotkeyText;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private ParticleSystem autocastParticles;

        private float cooldown;
        private bool onCooldown => cooldown != 0;
        private bool enableAutocast;
        private bool autocast;
        private bool subscribed;
        
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

        public void ToggleAutocast()
        {
#if UNITY_EDITOR
            if(PlayerManager.Instance.GodMode) return;
#endif
            enableAutocast = !enableAutocast;
            if(enableAutocast)
            {
                EnableAutocast();
                autocastParticles.Play();
                SubToEvents();
            }
            else
            {
                DisableAutocast();
                autocastParticles.Stop();
                UnsubFromEvents();
            }
        }

        private void DisableAutocast()
        {
            autocast = false;
        }

        private void EnableAutocast()
        {
            autocast = true;
            if(!TimeManager.IsDay) Activate();
        }
        
        public void Activate()
        {
            ActiveAbility activeAbility = (ActiveAbility) ability;
            if(onCooldown || !activeAbility.CanActivate()) return;
            
            activeAbility.Activate();
#if UNITY_EDITOR
            if(PlayerManager.Instance.GodMode) return;
#endif
            StartCoroutine(CooldownRoutine(activeAbility.Cooldown));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            cooldown = duration;
            while (cooldown > 0)
            {
                hotkeyText.text = Mathf.CeilToInt(cooldown).ToString();
                cooldownImage.fillAmount = cooldown / duration;
                cooldown -= Time.deltaTime;
                yield return null;
            }

            hotkeyText.text = ((ActiveMutation)Scriptable).KeyCode.ToString();
            cooldownImage.fillAmount = 0;
            cooldown = 0;
            if (autocast && !TimeManager.IsDay) Activate();
        }

        private void OnDayStart(int _)
        {
            if(enableAutocast) DisableAutocast();
        }

        private void OnNightStart(int _)
        {
            if (enableAutocast) EnableAutocast();
        }

        private void SubToEvents()
        {
            if(subscribed) return;
            subscribed = true;
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
        }

        private void UnsubFromEvents()
        {
            if(!subscribed) return;
            subscribed = false;
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }
        
        private void OnEnable()
        {
            if (Scriptable is null) return;
            if (onCooldown) StartCoroutine(CooldownRoutine(cooldown));
            if (enableAutocast)
            {
                autocastParticles.Play();
                SubToEvents();
            }
        }

        private void OnDisable()
        {
            if(Scriptable is null) return;
            hotkeyText.text = ((ActiveMutation)Scriptable).KeyCode.ToString();
            cooldownImage.fillAmount = 0;
            UnsubFromEvents();
        }
    }
}