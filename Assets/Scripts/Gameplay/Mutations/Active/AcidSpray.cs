using Definitions;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class AcidSpray : ActiveAbility, IDamageSource
    {
        [FormerlySerializedAs("particleSystem")] 
        [SerializeField] private ParticleSystem basicParticleSystem;
        [SerializeField] private ParticleSystem comboParticleSystem;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Spray angle")] 
        [SerializeField] private int angleLvl1;
        [SerializeField] private int angleLvl10;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;
        
        private float damage;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(basicParticleSystem.isPlaying) basicParticleSystem.Stop();
            if(comboParticleSystem.isPlaying) comboParticleSystem.Stop();
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = basicParticleSystem.emission;
            var shape = basicParticleSystem.shape;
            shape.angle = LerpLevel(angleLvl1, angleLvl10, lvl);
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, amount));
            emission = comboParticleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, amount * 2));

        }

        public override void Activate()
        {
            base.Activate();
            if (AttackController.IsInComboDash)
                comboParticleSystem.Play();
            else
                basicParticleSystem.Play();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if(damageable is IDamageableEnemy enemy)
                enemy.Damage(new DamageInstance(new DamageSource(this, collisionID),
                    GetAbilityDamage(damage),
                    PlayerPhysicsBody.Position,
                    damageColor: GlobalDefinitions.PoisonColor,
                    piercing: true));
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            float width = LerpLevel(angleLvl1, angleLvl10, lvl) * 2;
            float prevWidth = width;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = amount;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = dmg;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevWidth = LerpLevel(angleLvl1, angleLvl10, prevLvl) * 2;
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }

            return new object[]
            {
                cd,          width,             dmg,           amount, 
                cd - prevCd, width - prevWidth, dmg - prevDmg, amount - prevAmount, 
                0, 0
            };
        }
    }
}