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
        [SerializeField] private ParticleSystem parryParticles;

        public delegate void AttackEffectCollectionEvent(List<AttackEffect> effects);
        public static event AttackEffectCollectionEvent OnAttackEffectCollectionRequested;

        private readonly List<AttackEffect> effects = new();
        public bool IsActive { get; private set; }
        public static readonly List<AttackEffect> CurrentAttackEffects = new();
        private readonly Gradient defaultGradient = new Gradient().FastGradient(Color.white, Color.white);
        
        
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.gameObject.layer.Equals(GlobalDefinitions.EnemyAttackLayer) && 
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

        private void ApplyEffect(AttackEffect effect)
        {
            trailRenderer.colorGradient = effect.Color;
        }

        private void ResetEffects()
        {
            trailRenderer.colorGradient = defaultGradient;
            CurrentAttackEffects.Clear();
        }
        
        public void Enable()
        {
            OnAttackEffectCollectionRequested?.Invoke(effects);

            ResetEffects();

            if (effects.Count > 0)
            {
                CurrentAttackEffects.AddRange(effects);
                ApplyEffect(CurrentAttackEffects.OrderBy(_ => Random.value).First());
            }

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