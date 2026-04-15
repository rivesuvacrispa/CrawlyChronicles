using System.Collections.Generic;
using System.Linq;
using Gameplay.Player;
using Hitboxes;
using Timeline;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Passive
{
    public class GlowingBlood : BasicAbility, IDamageSource
    {
        [SerializeField] private new BulletParticleSystem particleSystem;
        [SerializeField] private new Light2D light;
        [SerializeField, MinMaxRange(0, 100)] private LevelInt amount = new LevelInt(25, 60);
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat damage = new LevelFloat(1, 3);
        [SerializeField, MinMaxRange(0, 1)] private LevelFloat lifetime = new LevelFloat(0.35f, 0.6f);
        
        
        private float currentDamage;
        private ParticleSystem.MainModule main;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDamage = damage.AtLvl(lvl);
            float currentLifetime = lifetime.AtLvl(lvl);
            light.pointLightOuterRadius = currentLifetime * 3;
            
            main = particleSystem.Particles.main;
            main.startLifetime = currentLifetime;
            
            particleSystem.SetBaseAmount(amount.AtLvl(lvl));
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(
                new DamageSource(this, collisionID),
                CalculateAbilityDamage(currentDamage),
                PlayerPhysicsBody.Position,
                piercing: true));
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
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void OnDayStart(int _)
        {
            particleSystem.Particles.Stop();
            light.enabled = false;
        }

        private void OnNightStart(int _)
        {
            particleSystem.Particles.Play();
            light.enabled = true;
        }
        

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new ILevelField[]
            {
                amount.UseKey(LevelFieldKeys.PARTICLES_AMOUNT),
                damage.UseKey(LevelFieldKeys.DAMAGE),
                lifetime.UseKey(LevelFieldKeys.EFFECT_RANGE).UseFormatter(new StatFormatter(multiplier: main.startSpeed.Evaluate(0)))
            };
        }
    }
}