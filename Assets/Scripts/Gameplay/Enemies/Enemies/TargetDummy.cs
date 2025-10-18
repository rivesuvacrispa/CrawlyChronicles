using System;
using System.Collections;
using Definitions;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects;
using SoundEffects;
using UI.Elements;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Enemies
{
    public class TargetDummy : MonoBehaviour, IDamageableEnemy, IImpactEffectAffectable
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
            audioController.PlayAction(scriptable.DeathAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            Start();
        }

        private void UpdateHealthbar()
        {
            Debug.Log($"Update healthbar: {CurrentHealth}/{scriptable.MaxHealth}, val: {Mathf.Clamp01(CurrentHealth / scriptable.MaxHealth)}");
            healthbar.SetValue(Mathf.Clamp01(CurrentHealth / scriptable.MaxHealth));
        }
        
        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            Die();
        }
        
        private void Knockback(Vector2 attacker, float force)
        {
            float kbResistance = PhysicsUtility.GetKnockbackResistance(scriptable.Mass);
            rb.AddClampedForceBackwards(attacker, force * (1 - kbResistance), ForceMode2D.Impulse);
        }
        
        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            UpdateHealthbar();
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false, AttackEffect effect = null)
        {
            hitbox.Hit();
            audioController.PlayAction(scriptable.HitAudio, pitch: SoundUtility.GetRandomPitchTwoSided(0.15f));
            bodyPainter.Paint(new Gradient().FastGradient(damageColor, scriptable.BodyColor), GlobalDefinitions.EnemyImmunityDuration);
            if (knockback > 0)
            {
                Knockback(position, knockback);
            }
        }

        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            if(!hitbox.Dead) effectController.AddEffect<T>(data);
        }

        private void OnDestroy()
        {
            OnProviderDestroy?.Invoke(this);
        }
    }
}