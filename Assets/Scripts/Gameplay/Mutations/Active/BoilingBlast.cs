using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Active
{
    public class BoilingBlast : ActiveAbility, IDamageSource
    {
        [SerializeField] private BulletParticleSystem basicParticles;
        [SerializeField] private BulletParticleSystem comboParticles;
        [SerializeField, MinMaxRange(0, 100)] private LevelInt amount = new LevelInt(new Vector2Int(25, 75));
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat stunDuration = new LevelFloat(new Vector2(1, 3));
        [SerializeField, MinMaxRange(0, 10)] private LevelFloat knockback = new LevelFloat(new Vector2(4, 10));
        [SerializeField, MinMaxRange(0, 10)] private LevelFloat damage = new LevelFloat(new Vector2(3, 10));



        private float currentDamage;
        private float currentKnockback;
        private float currentStunDurtation;
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(basicParticles.Particles.isPlaying) basicParticles.Particles.Stop();
            currentDamage = damage.AtLvl(lvl);
            currentStunDurtation = stunDuration.AtLvl(lvl);
            currentKnockback = knockback.AtLvl(lvl);
            
            float currentAmount = amount.AtLvl(lvl);
            basicParticles.SetBaseAmount(currentAmount);
            comboParticles.SetBaseAmount(currentAmount * 2);
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(
                new DamageInstance(new DamageSource(this),
                    CalculateAbilityDamage(currentDamage),
                    PlayerPhysicsBody.Position,
                    currentKnockback,
                    currentStunDurtation,
                    Color.orange));
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            if (AttackController.IsInComboDash)
                comboParticles.Particles.Play();
            else
                basicParticles.Particles.Play();
        }
        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                amount.UseKey(LevelFieldKeys.PARTICLES_AMOUNT),
                damage.UseKey(LevelFieldKeys.DAMAGE),
                knockback.UseKey(LevelFieldKeys.KNOCKBACK),
                stunDuration.UseKey(LevelFieldKeys.STUN_DURATION).UseFormatter(StatFormatter.SECONDS)
            };
        }
    }
}