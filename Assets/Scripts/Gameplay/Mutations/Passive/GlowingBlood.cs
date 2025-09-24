using Gameplay.Player;
using Timeline;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util.Interfaces;

namespace Gameplay.Mutations.Passive
{
    public class GlowingBlood : BasicAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private new Light2D light;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Damage")] 
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;
        [Header("Particles lifetime")] 
        [SerializeField] private float lifetimeLvl1;
        [SerializeField] private float lifetimeLvl10;
        
        
        private float damage;


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            float lifetime = LerpLevel(lifetimeLvl1, lifetimeLvl10, lvl);
            light.pointLightOuterRadius = lifetime * 3;
            var emission = particleSystem.emission;
            var main = particleSystem.main;
            emission.rateOverTime = LerpLevel(amountLvl1, amountLvl10, lvl);
            main.startLifetime = lifetime;
        }
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out IDamageableEnemy enemy))
                enemy.Damage(
                    GetAbilityDamage(damage),
                    PlayerMovement.Position,
                    0,
                    0,
                    Color.white,
                    true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(TimeManager.IsDay) OnDayStart(0);
            else OnNightStart(0);
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubFromEvents();
        }

        private void OnDestroy() => UnsubFromEvents();

        private void UnsubFromEvents()
        {
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void OnDayStart(int _)
        {
            particleSystem.Stop();
            light.enabled = false;
        }

        private void OnNightStart(int _)
        {
            particleSystem.Play();
            light.enabled = true;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            float life = LerpLevel(lifetimeLvl1, lifetimeLvl10, lvl) * 3;
            float prevLife = life;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = amount;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = dmg;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevLife = LerpLevel(lifetimeLvl1, lifetimeLvl10, prevLvl) * 3;
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            
            var args = new object[]
            {
                 amount,              dmg,           life,           
                 amount - prevAmount, dmg - prevDmg, life - prevLife,
            };
            
            return scriptable.GetStatDescription(args);
        }
    }
}