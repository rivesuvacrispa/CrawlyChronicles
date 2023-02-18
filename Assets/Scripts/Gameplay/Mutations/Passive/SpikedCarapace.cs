using System;
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
            probability = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, 
                LerpLevel(amountLvl1, amountLvl10, lvl)));
        }

        private void Activate()
        {
            particleSystem.Play();
        }
        
        private void OnDamageTaken(int damage)
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
    }
}