using Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class BoilingBlast : ActiveAbility
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Stun duration")] 
        [SerializeField] private float stunLvl1;
        [SerializeField] private float stunLvl10;
        [Header("Knockback")] 
        [SerializeField] private float knockbackLvl1;
        [SerializeField] private float knockbackLvl10;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;


        private float damage;
        private float knockback;
        private float stun;
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            stun = LerpLevel(stunLvl1, stunLvl10, lvl);
            knockback = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, LerpLevel(amountLvl1, amountLvl10, lvl)));
        }
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out IDamageableEnemy enemy))
                enemy.Damage(
                    GetAbilityDamage(damage),
                    PlayerMovement.Position,
                    knockback,
                    stun,
                    Color.white);
        }
        
        public override void Activate() => particleSystem.Play();
        public override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = amount;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = dmg;            
            float kb = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            float prevKb = kb;   
            float stunDur = LerpLevel(stunLvl1, stunLvl10, lvl);
            float prevStunDur = stunDur;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevStunDur = LerpLevel(stunLvl1, stunLvl10, prevLvl);
                prevKb = LerpLevel(knockbackLvl1, knockbackLvl10, prevLvl);
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            return new object[] { 
                cd,          amount,              stunDur,               dmg,           kb, 
                cd - prevCd, amount - prevAmount, stunDur - prevStunDur, dmg - prevDmg, kb - prevKb };
        }
    }
}