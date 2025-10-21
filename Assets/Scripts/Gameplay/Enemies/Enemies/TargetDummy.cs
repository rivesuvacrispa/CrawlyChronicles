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
        public bool Immune => hitbox.Immune;
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
        
        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            Die();
        }
        
        private void Knockback(Vector2 attacker, float force)
        {
            float kbResistance = PhysicsUtility.GetKnockbackResistance(scriptable.Mass);
            rb.AddClampedForceBackwards(attacker, force * (1 - kbResistance), ForceMode2D.Impulse);
        }
        
        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDamageTaken?.Invoke(this, damage);
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            hitbox.Hit();
            audioController.PlayAction(scriptable.HitAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            bodyPainter.Paint(new Gradient().FastGradient(damageColor, scriptable.BodyColor), GlobalDefinitions.EnemyImmunityDuration);
            if (knockback > 0)
            {
                Knockback(position, knockback);
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