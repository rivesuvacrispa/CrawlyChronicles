using System.Collections;
using Definitions;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Bosses.Centipede
{
    [RequireComponent(typeof(Collider2D))]
    public class CentipedeHitbox : MonoBehaviour
    {
        public CentipedeFragment Fragment { get; set; }
        private new Collider2D collider;


        public bool Enabled => collider.enabled;
        
        private void Awake() => collider = GetComponent<Collider2D>();

        // Player damage is not coded with IEnemyAttack because player hitbox 
        // does not detect trigger entering
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
                ((IDamageable)PlayerManager.Instance).Damage(CentipedeBoss.ContactDamage, transform.position,
                    CentipedeDefinitions.Knockback, 0, default);
            else
                ((IDamageable)Fragment).Damage(
                PlayerManager.PlayerStats.AttackDamage, default, 0, 
                0, default, false, PlayerAttack.CurrentAttackEffects);
        }

        public void Hit() => StartCoroutine(ImmunityRoutine());
        public void Die() => gameObject.SetActive(false);

        private IEnumerator ImmunityRoutine()
        {
            collider.enabled = false;
            yield return new WaitForSeconds(GlobalDefinitions.EnemyImmunityDuration);
            collider.enabled = true;
        }
    }
}