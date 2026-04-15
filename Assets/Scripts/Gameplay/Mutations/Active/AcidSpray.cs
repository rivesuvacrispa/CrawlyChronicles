using Definitions;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using UnityEngine.Serialization;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Active
{
    public class AcidSpray : ActiveAbility, IDamageSource
    {
        [FormerlySerializedAs("particleSystem")]
        [SerializeField] private BulletParticleSystem basicParticleSystem;

        [SerializeField] private BulletParticleSystem comboParticleSystem;
        [SerializeField, MinMaxRange(0, 100)] private LevelInt amount = new LevelInt(new Vector2Int(20, 50));
        [SerializeField, MinMaxRange(0, 90)] private LevelInt angle = new LevelInt(new Vector2Int(15, 30));
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat damage = new LevelFloat(new Vector2(0.2f, 1.5f));

        private float currentDamage;


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if (basicParticleSystem.Particles.isPlaying) basicParticleSystem.Particles.Stop();
            if (comboParticleSystem.Particles.isPlaying) comboParticleSystem.Particles.Stop();

            currentDamage = damage.AtLvl(lvl);

            basicParticleSystem.SetShapeAngle(angle.AtLvl(lvl));

            float currentAmount = amount.AtLvl(lvl);
            basicParticleSystem.SetBaseAmount(currentAmount);
            comboParticleSystem.SetBaseAmount(currentAmount * 2);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            if (AttackController.IsInComboDash)
                comboParticleSystem.Particles.Play();
            else
                basicParticleSystem.Particles.Play();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is IDamageableEnemy enemy)
                enemy.Damage(new DamageInstance(new DamageSource(this, collisionID),
                    CalculateAbilityDamage(currentDamage),
                    PlayerPhysicsBody.Position,
                    damageColor: GlobalDefinitions.PoisonColor,
                    piercing: true));
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                angle.UseKey(LevelFieldKeys.SPRAY_ANGLE).UseFormatter(StatFormatter.DEGREE.WithMultiplier(2f)),
                amount.UseKey(LevelFieldKeys.PARTICLES_AMOUNT),
                damage.UseKey(LevelFieldKeys.DAMAGE)
            };
        }
    }
}