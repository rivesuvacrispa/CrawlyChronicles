using System;
using System.Collections.Generic;
using System.Linq;
using Definitions;
using Gameplay.Effects.DamageText;
using Gameplay.Enemies;
using Gameplay.Mutations.AttackEffects;
using Hitboxes;
using SoundEffects;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class BasePlayerAttack : MonoBehaviour, IDamageSource, ICanChangeDamageText
    {
        [SerializeField] private BaseAudioSource audioSource;
        [SerializeField] protected TrailRenderer trailRenderer;

        public delegate void AttackEffectCollectionEvent(List<AttackEffect> effects);
        public static event AttackEffectCollectionEvent OnAttackEffectCollectionRequested;

        public bool IsActive { get; private set; }
        private readonly List<AttackEffect> effects = new();
        private readonly List<AttackEffect> currentAttackEffects = new(4);
        private readonly Gradient defaultGradient = new Gradient().FastGradient(Color.white, Color.white);
        private int attackCounter;
        private int GetAttackID() => HashCode.Combine(gameObject.GetHashCode(), attackCounter);
        private float currentBonusDamage;


        
        private void Awake()
        {
            PlayerSizeManager.OnSizeChanged += OnPlayerSizeChanged;
        }

        private void OnPlayerSizeChanged(float size)
        {
            trailRenderer.widthMultiplier = size;
        }

        public DamageInstance CreateDamageInstance()
        {
            attackCounter++;
            int id = GetAttackID();
            
            return new DamageInstance(
                new DamageSource(this, id),
                PlayerManager.PlayerStats.AttackDamage + currentBonusDamage,
                transform.position,
                PlayerManager.PlayerStats.AttackPower,
                0.35f,
                Color.white,
                effects: currentAttackEffects);
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.gameObject.layer.Equals(GlobalDefinitions.EnemyAttackLayer) && 
                col.gameObject.TryGetComponent(out Enemy enemy))
            {
                PlayerAudioController.Instance.PlayReckoning();
                Vector2 point = col.contacts[0].point;
                float force = PlayerManager.PlayerStats.AttackPower;
                if(!AttackController.IsInComboDash) 
                    PlayerManager.Instance.Knockback(point, force);
                enemy.Reckon(point, force);
                
                if (col.contacts.Length != 0)
                {
                    PlayerParryParticles.Play(col.contacts.First().point);
                }
            }
        }

        private void ApplyRandomEffectTrail()
        {
            var eff = currentAttackEffects.OrderBy(_ => Random.value).FirstOrDefault();
            trailRenderer.colorGradient = eff is null ? defaultGradient : eff.Color;
        }

        private void ResetEffects()
        {
            trailRenderer.colorGradient = defaultGradient;
            currentAttackEffects.Clear();
        }

        public void AddBonusDamage(float dmg) => currentBonusDamage += dmg;

        public void RemoveEffect(AttackEffect effect)
        {
            if (currentAttackEffects.Remove(effect))
                ApplyRandomEffectTrail();
        }
        
        public void Enable()
        {
            OnAttackEffectCollectionRequested?.Invoke(effects);

            ResetEffects();

            currentAttackEffects.AddRange(effects);
            foreach (AttackEffect effect in effects) effect.Apply(this);
            ApplyRandomEffectTrail();

            IsActive = true;
            gameObject.SetActive(true);
            audioSource.Play(pitch: 1f / PlayerSizeManager.CurrentSize);
        }

        public void Disable()
        {
            effects.Clear();
            IsActive = false;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            currentBonusDamage = 0f;
        }

        
        // ICanChangeDamageText
        public bool ShouldChangeDamageText()
        {
            return currentAttackEffects.Exists(eff => eff is ICanChangeDamageText);
        }

        public DamageTextProperties GetDamageTextProperties()
        {
            var eff = currentAttackEffects.FirstOrDefault(eff => eff is ICanChangeDamageText);
            return ((ICanChangeDamageText)eff)?.GetDamageTextProperties();
        }
    }
}