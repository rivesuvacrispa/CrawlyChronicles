using Definitions;
using Gameplay.Mutations.EntityEffects;
using SoundEffects;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Enemies
{
    public class TargetDummy : MonoBehaviour, IDamageableEnemy, IEffectAffectable
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private AudioController audioController;
        [SerializeField] private DamageableEnemyHitbox hitbox;
        [SerializeField] private Scriptable.Enemy scriptable;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private EffectController effectController;

        
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => scriptable.HealthbarOffsetY;
        public float HealthbarWidth => scriptable.HealthbarWidth;
        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool ImmuneToSource(DamageSource source) => hitbox.ImmuneToSource(source);
        public float Armor => scriptable.Armor;
        public float CurrentHealth { get; set; }
        public float MaxHealth => scriptable.MaxHealth;


        private void Start()
        {
            CurrentHealth = scriptable.MaxHealth;
        }

        private void Die()
        {
            audioController.PlayAction(scriptable.DeathAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            Start();
        }
        
        public void OnLethalHit(DamageInstance instance)
        {
            Die();
        }
        
        private void Knockback(Vector2 attacker, float force)
        {
            float kbResistance = PhysicsUtility.GetKnockbackResistance(scriptable.Mass);
            rb.AddClampedForceBackwards(attacker, force * (1 - kbResistance), ForceMode2D.Impulse);
        }
        
        public void OnBeforeHit(DamageInstance instance)
        {
            OnDamageTaken?.Invoke(this, instance.Damage);
        }

        public void OnHit(DamageInstance instance)
        {
            hitbox.Hit(instance);
            audioController.PlayAction(scriptable.HitAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            bodyPainter.Paint(new Gradient().FastGradient(instance.damageColor, scriptable.BodyColor), GlobalDefinitions.EnemyImmunityDuration);
            if (instance.knockback > 0)
            {
                Knockback(instance.position, instance.knockback);
            }
        }
        
        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke(this);
        }

        
        // IEffectAffectable
        public EffectController EffectController => effectController;
        public bool CanApplyEffect => !hitbox.Dead;
    }
}