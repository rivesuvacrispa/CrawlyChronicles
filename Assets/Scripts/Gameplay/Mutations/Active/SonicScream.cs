using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;

namespace Gameplay.Mutations.Active
{
    public class SonicScream : ActiveAbility, IDamageSource
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField, MinMaxRange(1, 5)] private LevelInt wavesAmount = new LevelInt(1, 3);
        [SerializeField, MinMaxRange(0f, 1f)] private LevelFloat lifetime = new LevelFloat(0.35f, 0.6f);
        [SerializeField, MinMaxRange(0f, 5f)] private LevelFloat stunDuration = new LevelFloat(1, 3);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat knockback = new LevelFloat(3f, 6f);
        
        private float currentStunDuration;
        private float currentKnockback;
        private ParticleSystem.MainModule main;
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if(particleSystem.isPlaying) particleSystem.Stop();
            currentStunDuration = stunDuration.AtLvl(lvl);
            currentKnockback = knockback.AtLvl(lvl);
            var emission = particleSystem.emission;
            main = particleSystem.main;
            int waves = wavesAmount.AtLvlFloor(lvl);
            emission.SetBurst(0, new ParticleSystem.Burst(0, 15, 15, waves, 0.2f));
            float currentLifetime = lifetime.AtLvl(lvl);
            main.startLifetime = new ParticleSystem.MinMaxCurve(currentLifetime, currentLifetime + 0.25f);
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            damageable.Damage(new DamageInstance(new DamageSource(this), 
                0,
                PlayerPhysicsBody.Position,
                currentKnockback,
                currentStunDuration,
                Color.pink));
        }
        
        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            particleSystem.Play();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                wavesAmount.UseKey(LevelFieldKeys.WAVES_AMOUNT),
                lifetime.UseKey(LevelFieldKeys.WAVES_LENGTH).UseFormatter(new StatFormatter(multiplier: main.startLifetime.constant)),
                stunDuration.UseKey(LevelFieldKeys.STUN_DURATION).UseFormatter(StatFormatter.SECONDS),
                knockback.UseKey(LevelFieldKeys.KNOCKBACK)
            };
        }
    }
}