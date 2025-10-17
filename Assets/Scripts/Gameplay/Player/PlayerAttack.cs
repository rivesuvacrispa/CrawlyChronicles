using System.Collections.Generic;
using System.Linq;
using Definitions;
using Gameplay.Enemies;
using Gameplay.Mutations.AttackEffects;
using SoundEffects;
using UnityEngine;
using Util;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField, Range(0, 1)] private float basicEffectChance;
        [SerializeField] private ParticleSystem parryParticles;

        public delegate void AttackEffectCollectionEvent(List<AttackEffect> effects);
        public static event AttackEffectCollectionEvent OnAttackEffectCollectionRequested;

        private readonly List<AttackEffect> effects = new();
        public bool IsActive { get; private set; }
        public static AttackEffect CurrentAttackEffect { get; private set;  }
        private readonly Gradient defaultGradient = new Gradient().FastGradient(Color.white, Color.white);
        
        
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.gameObject.layer.Equals(GlobalDefinitions.EnemyAttackLayerMask) && 
                col.gameObject.TryGetComponent(out Enemy enemy))
            {
                PlayerAudioController.Instance.PlayReckoning();
                Vector2 point = col.contacts[0].point;
                float force = PlayerManager.PlayerStats.AttackPower;
                if(!AttackController.IsInComboDash) movement.Knockback(point, force);
                enemy.Reckon(point, force);
                parryParticles.transform.position = col.contacts.First().point;
                parryParticles.Play();
            }
        }

        private bool GetRandomEffect(out AttackEffect effect)
        {
            effect = null;
            if (effects.Count == 0 ||
                Random.value > basicEffectChance * PlayerManager.PlayerStats.PassiveProcRate) 
                return false;
            effect = effects[Random.Range(0, effects.Count)];
            return true;
        }

        private void ApplyEffect(AttackEffect effect)
        {
            trailRenderer.colorGradient = effect.Color;
            CurrentAttackEffect = effect;
        }

        private void ResetEffect()
        {
            trailRenderer.colorGradient = defaultGradient;
            CurrentAttackEffect = null;
        }
        
        public void Enable()
        {
            OnAttackEffectCollectionRequested?.Invoke(effects);
            AttackEffect guaranteed = effects.FirstOrDefault(e => e.Guaranteed);
            
            if (guaranteed is not null) 
                ApplyEffect(guaranteed);
            else if (GetRandomEffect(out var effect)) 
                ApplyEffect(effect);
            else 
                ResetEffect();
            
            IsActive = true;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            effects.Clear();
            IsActive = false;
            gameObject.SetActive(false);
        }
    }
}