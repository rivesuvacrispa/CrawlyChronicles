using System;
using System.Text;
using Gameplay.Enemies;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Abilities.Passive
{
    public class SpikedCarapace : BasicAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private int damageCap;
        [Header("Trigger chance")]
        [SerializeField, Range(0, 1)] private float probabilityLvl1;
        [SerializeField, Range(0, 1)] private float probabilityLvl10;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)] private float knockbackPower = 0.5f;

        private float probability;
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            probability = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, 
                LerpLevel(amountLvl1, amountLvl10, lvl)));
        }

        private void Activate()
        {
            particleSystem.Play();
        }
        
        private void OnDamageTaken(float damage)
        {
            if(damage >= damageCap || Random.value <= probability) Activate();
        }
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.Damage(1, knockbackPower, stunDuration);
            }
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerHitbox.OnDamageTaken -= OnDamageTaken;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerHitbox.OnDamageTaken += OnDamageTaken;
        }
        
        public override string GetLevelDescription(int lvl)
        {
            float prob = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=orange>").Append("Trigger chance").Append(": ").Append("</color>").Append(prob.ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Damage cap").Append(": ").Append("</color>").Append(damageCap.ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Bullets amount").Append(": ").Append("</color>").Append(amount.ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Stun duration").Append(": ").Append("</color>").Append(stunDuration.ToString("n2")).Append("\n");
            sb.Append("<color=orange>").Append("Knockback").Append(": ").Append("</color>").Append(knockbackPower.ToString("n2")).Append("\n");
            return sb.ToString();
        }
    }
}