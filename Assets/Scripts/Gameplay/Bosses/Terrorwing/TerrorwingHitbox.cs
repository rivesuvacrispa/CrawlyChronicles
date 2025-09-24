﻿using System.Collections;
using Definitions;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Bosses.Terrorwing
{
    [RequireComponent(typeof(Collider2D))]
    public class TerrorwingHitbox : MonoBehaviour
    {
        [SerializeField] private TerrorwingClone terrorwingClone;
        
        private Collider2D hitboxCollider;

        private bool dead;
        public bool Immune => !hitboxCollider.enabled || dead;

        private void Awake() => hitboxCollider = GetComponent<Collider2D>();

        
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!ContactDamage(col)) 
                terrorwingClone.Damage(
                    PlayerManager.PlayerStats.AttackDamage,
                    effect: PlayerAttack.CurrentAttackEffect);
        }

        private bool ContactDamage(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
            {
                ((IDamageable)PlayerManager.Instance).Damage(
                    TerrorwingDefinitions.ContactDamage,
                    transform.position,
                    5, 0, default);
                return true;
            }

            return false;
        }

        public void Hit() => StartCoroutine(ImmunityRoutine());
        
        public void Die()
        {
            StopAllCoroutines();
            dead = true;
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            StopAllCoroutines();
            hitboxCollider.enabled = true;
        }

        public void Disable()
        {
            StopAllCoroutines();
            hitboxCollider.enabled = false;
        }

        private IEnumerator ImmunityRoutine()
        {
            hitboxCollider.enabled = false;
            yield return new WaitForSeconds(GlobalDefinitions.EnemyImmunityDuration);
            hitboxCollider.enabled = true;
        }
    }
}