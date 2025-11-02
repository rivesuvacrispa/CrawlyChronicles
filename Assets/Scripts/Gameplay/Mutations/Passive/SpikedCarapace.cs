using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Passive
{
    public class SpikedCarapace : BasicAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Trigger chance")]
        [SerializeField, Range(0, 1)] private float probabilityLvl1;
        [SerializeField, Range(0, 1)] private float probabilityLvl10;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)] private float knockbackPower = 0.5f;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        private float procRate;
        private float damage;
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            procRate = LerpLevel(probabilityLvl1, probabilityLvl10, lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, 
                LerpLevel(amountLvl1, amountLvl10, lvl)));
        }

        private void Activate()
        {
            particleSystem.Play();
        }
        
        private void OnStruck()
        {
            if(Random.value <= GetPassiveProcRate(procRate)) Activate();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(
                    new DamageSource(this, collisionID),
                GetAbilityDamage(damage),
                    PlayerPhysicsBody.Position,
                    knockbackPower,
                    stunDuration,
                    Color.white)
                );
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerManager.OnStruck -= OnStruck;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerManager.OnStruck += OnStruck;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            int prob = (int) (LerpLevel(probabilityLvl1, probabilityLvl10, lvl) * 100);
            int prevProb = prob;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = amount;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = dmg;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevProb = (int) (LerpLevel(probabilityLvl1, probabilityLvl10, prevLvl) * 100);
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            
            var args = new object[]
            {
                prob,            amount,              dmg,
                prob - prevProb, amount - prevAmount, dmg - prevDmg,
                stunDuration, knockbackPower
            };
            return scriptable.GetStatDescription(args);
        }
    }
}