using System.Collections;
using Definitions;
using Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageableEnemyHitbox : MonoBehaviour
    {
        [SerializeField] private Component damagableComponent;

        private new Collider2D collider;
        private IDamageableEnemy enemy;

        public bool Dead { get; private set; }
        public bool Immune => !collider.enabled || Dead;

        private void Awake()
        {
            collider = GetComponent<Collider2D>();
            if (damagableComponent is IDamageableEnemy e)
                enemy = e;
            else
            {
                Debug.LogError($"Component {damagableComponent.name} is not IDamageableEnemy");
                enabled = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D _)
        {
            enemy.Damage(
                PlayerManager.PlayerStats.AttackDamage,
                PlayerMovement.Position,
                PlayerManager.PlayerStats.AttackPower,
                0.35f, 
                Color.white,
                effect: PlayerAttack.CurrentAttackEffect);
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            collider.enabled = true;
        }

        public void Disable()
        {
            StopAllCoroutines();
            collider.enabled = false;
        }

        public void Die()
        {
            Dead = true;
            StopAllCoroutines();
            Disable();
        }

        public void Hit() => StartCoroutine(ImmunityRoutine());

        private IEnumerator ImmunityRoutine()
        { 
            collider.enabled = false;
            yield return new WaitForSeconds(GlobalDefinitions.EnemyImmunityDuration);
            collider.enabled = true;
        }
    }
}