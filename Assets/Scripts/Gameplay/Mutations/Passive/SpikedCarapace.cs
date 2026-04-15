using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations.Passive
{
    public class SpikedCarapace : BasicAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField, MinMaxRange(0, 1)] private LevelFloat procRate = new LevelFloat(0.25f, 0.5f);
        [SerializeField, MinMaxRange(0, 100)] private LevelInt amount = new LevelInt(15, 40);
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat damage = new LevelFloat(1, 4);
        [SerializeField] private LevelConst stunDuration = new LevelConst(0.5f);
        [SerializeField] private LevelConst knockbackPower = new LevelConst(0.5f);

        private float currentProcRate;
        private float currentDamage;


        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                procRate.UseKey(LevelFieldKeys.PROC_CHANCE).UseFormatter(StatFormatter.PERCENT),
                amount.UseKey(LevelFieldKeys.PARTICLES_AMOUNT),
                damage.UseKey(LevelFieldKeys.DAMAGE),
                stunDuration.UseKey(LevelFieldKeys.STUN_DURATION),
                knockbackPower.UseKey(LevelFieldKeys.KNOCKBACK)
            };
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            currentProcRate = procRate.AtLvl(lvl);
            currentDamage = damage.AtLvl(lvl);
            var emission = particleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, amount.AtLvl(lvl)));
        }

        private void Activate()
        {
            particleSystem.Play();
        }
        
        private void OnStruck()
        {
            if(TryProc(currentProcRate)) 
                Activate();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(
                    new DamageSource(this, collisionID),
                CalculateAbilityDamage(currentDamage),
                    PlayerPhysicsBody.Position,
                    knockbackPower.Value,
                    stunDuration.Value,
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
    }
}