using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Active
{
    public class SpikeFan : ActiveAbility, IDamageSource
    {
        [SerializeField] private new BulletParticleSystem particleSystem;
        [SerializeField, MinMaxRange(0, 50)] private LevelInt amount = new LevelInt(20, 40);
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat duration = new LevelFloat(1, 3);
        [SerializeField] private LevelConst stunDuration = new LevelConst(0.5f);
        [SerializeField]  private LevelConst knockbackPower = new LevelConst(0.5f);
        [SerializeField, MinMaxRange(0f, 5f)] private LevelFloat damage = new LevelFloat(1f, 3f);

        
        private float currentDamage;
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.Particles.isPlaying) particleSystem.Particles.Stop();
            currentDamage = damage.AtLvl(lvl);
            particleSystem.SetBaseAmount(amount.AtLvl(lvl));
            particleSystem.SetDuration(duration.AtLvl(lvl));
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(new DamageSource(this, collisionID), 
                CalculateAbilityDamage(currentDamage),
                PlayerPhysicsBody.Position,
                knockbackPower.Value,
                stunDuration.Value,
                Color.white));
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            if (particleSystem.Particles.isPlaying) particleSystem.Particles.time = 0;
            else particleSystem.Particles.Play();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                duration.UseKey(LevelFieldKeys.DURATION).UseFormatter(StatFormatter.SECONDS),
                amount.UseKey(LevelFieldKeys.PARTICLES_AMOUNT),
                damage.UseKey(LevelFieldKeys.DAMAGE)
            };
        }
    }
}