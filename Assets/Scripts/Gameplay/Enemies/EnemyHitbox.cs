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


        
        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnCollisionEnter2D(Collision2D _)
        {
            float damage = enemy.Damage(Manager.PlayerStats.AttackDamage, 
                Manager.PlayerStats.AttackPower, 0.35f, Color.white);
            if(PlayerAttack.CurrentAttackEffect is not null)
                PlayerAttack.CurrentAttackEffect.Impact(enemy, damage);
            StartCoroutine(ImmunityRoutine());
        }

        public void Enable() => collider.enabled = true;

        public void Disable() => collider.enabled = false;
        
        private IEnumerator ImmunityRoutine()
        { 
            Disable();
            yield return new WaitForSeconds(GlobalDefinitions.EnemyImmunityDuration);
            Enable();
        }
    }
}