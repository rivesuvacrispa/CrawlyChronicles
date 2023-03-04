using System.Collections;
using Definitions;
using Player;
using UnityEngine;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private new Collider2D collider;


        public bool Enabled => collider.enabled;
        
        
        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnCollisionEnter2D(Collision2D _)
        {
            float damage = enemy.Damage(
                Manager.PlayerStats.AttackDamage,
                Movement.Position,
                Manager.PlayerStats.AttackPower,
                0.35f, 
                Color.white);
            
            if(PlayerAttack.CurrentAttackEffect is not null)
                PlayerAttack.CurrentAttackEffect.Impact(enemy, damage);
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