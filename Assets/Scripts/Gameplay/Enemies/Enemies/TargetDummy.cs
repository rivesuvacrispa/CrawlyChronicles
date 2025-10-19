using System;
using System.Collections;
using Definitions;
using Gameplay.Mutations.EntityEffects;
using SoundEffects;
using UI.Elements;
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
        public bool Immune => hitbox.Immune;
        public float Armor => scriptable.Armor;
        public float CurrentHealth { get; set; }
        private Healthbar healthbar;

        private void Awake()
        {
            healthbar = HealthbarPool.Instance.Create(this);
        }

        private void Start()
        {
            CurrentHealth = scriptable.MaxHealth;
            UpdateHealthbar();
        }

        private void Die()
        {
            OnDeath?.Invoke(this);
            audioController.PlayAction(scriptable.DeathAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            Start();
        }

        private void UpdateHealthbar()
        {
            healthbar.SetValue(Mathf.Clamp01(CurrentHealth / scriptable.MaxHealth));
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
            UpdateHealthbar();
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