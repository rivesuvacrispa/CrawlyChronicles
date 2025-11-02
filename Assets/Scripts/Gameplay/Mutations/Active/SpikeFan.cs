using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class SpikeFan : ActiveAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Fan duration")] 
        [SerializeField] private float durationLvl1;
        [SerializeField] private float durationLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)]  private float knockbackPower = 0.5f;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        
        private float damage;
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = particleSystem.emission;
            var main = particleSystem.main;
            emission.rateOverTime = LerpLevel(amountLvl1, amountLvl10, lvl);
            main.duration = LerpLevel(durationLvl1, durationLvl10, lvl);
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(new DamageSource(this, collisionID), 
                GetAbilityDamage(damage),
                PlayerPhysicsBody.Position,
                knockbackPower,
                stunDuration,
                Color.white));
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            if (particleSystem.isPlaying) particleSystem.time = 0;
            else particleSystem.Play();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            float dur = LerpLevel(durationLvl1, durationLvl10, lvl);
            float prevDur = dur;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = amount;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = dmg;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevDur = LerpLevel(durationLvl1, durationLvl10, prevLvl);
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            
            return new object[]
            {
                cd,          dur,           amount,              dmg, 
                cd - prevCd, dur - prevDur, amount - prevAmount, dmg - prevDmg,
                stunDuration, knockbackPower
            };
        }
    }
}