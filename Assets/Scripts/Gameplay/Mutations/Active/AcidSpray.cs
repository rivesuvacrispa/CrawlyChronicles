using System.Text;
using Definitions;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Util.Interfaces;

namespace Gameplay.Abilities.Active
{
    public class AcidSpray : ActiveAbility
    {
        [FormerlySerializedAs("particleSystem")] 
        [SerializeField] private ParticleSystem basicParticleSystem;
        [SerializeField] private ParticleSystem comboParticleSystem;
        [SerializeField] private ParticleCollisionProvider provider1;
        [SerializeField] private ParticleCollisionProvider provider2;
        [Header("Particles amount")] 
        [SerializeField] private int amountLvl1;
        [SerializeField] private int amountLvl10;
        [Header("Spray angle")] 
        [SerializeField] private int angleLvl1;
        [SerializeField] private int angleLvl10;
        [Header("Stun and knockback")] 
        [SerializeField, Range(0, 1)] private float stunDuration = 0.5f;
        [SerializeField, Range(0, 10)]  private float knockbackPower = 0.5f;
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        
        private float damage;

        protected override void Start()
        {
            base.Start();
            provider1.OnCollision += OnBulletCollision;
            provider2.OnCollision += OnBulletCollision;
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(basicParticleSystem.isPlaying) basicParticleSystem.Stop();
            if(comboParticleSystem.isPlaying) comboParticleSystem.Stop();
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            var emission = basicParticleSystem.emission;
            var shape = basicParticleSystem.shape;
            shape.angle = LerpLevel(angleLvl1, angleLvl10, lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, LerpLevel(amountLvl1, amountLvl10, lvl)));
            emission = comboParticleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, 2 *LerpLevel(amountLvl1, amountLvl10, lvl)));

        }

        public override void Activate()
        {
            if (AttackController.IsInComboDash)
                comboParticleSystem.Play();
            else
                basicParticleSystem.Play();
        }
        
        private void OnBulletCollision(IDamageable damageable)
        {
            if(damageable is IDamageableEnemy enemy)
                enemy.Damage(
                    GetAbilityDamage(damage), 
                    PlayerMovement.Position,
                    knockbackPower, 
                    stunDuration, 
                    GlobalDefinitions.PoisonColor, 
                    ignoreArmor: true);
        }

        private void OnDestroy()
        {
            provider1.OnCollision -= OnBulletCollision;
            provider2.OnCollision -= OnBulletCollision;
        }

        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float width = LerpLevel(angleLvl1, angleLvl10, lvl) * 2;
            float prevWidth = 0;
            float amount = LerpLevel(amountLvl1, amountLvl10, lvl);
            float prevAmount = 0;
            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevWidth = LerpLevel(angleLvl1, angleLvl10, prevLvl) * 2;
                prevAmount = LerpLevel(amountLvl1, amountLvl10, prevLvl);
                prevDmg = LerpLevel(damageLvl1, damageLvl10, prevLvl);
            }
            
            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false, suffix: "s");
            sb.AddAbilityLine("Spray width", width, prevWidth);
            sb.AddAbilityLine("Spray amount", amount, prevAmount);
            sb.AddAbilityLine("Spray damage", dmg, prevDmg);
            sb.AddAbilityLine("Stun duration", stunDuration, 0);
            sb.AddAbilityLine("Knockback", knockbackPower, 0);
            
            return sb.ToString();
        }
    }
}