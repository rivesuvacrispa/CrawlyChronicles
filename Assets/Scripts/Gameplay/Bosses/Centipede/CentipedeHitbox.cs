using System.Collections;
using Definitions;
using Gameplay.Enemies;
using Player;
using UnityEngine;

namespace Scripts.Gameplay.Bosses.Centipede
{
    [RequireComponent(typeof(Collider2D))]
    public class CentipedeHitbox : MonoBehaviour
    {
        public CentipedeFragment Fragment { get; set; }
        private new Collider2D collider;


        public bool Enabled => collider.enabled;
        
        private void Awake() => collider = GetComponent<Collider2D>();

        // Player damage is not realized with IEnemyAttack because player hitbox 
        // does not detect trigger entering
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
                Manager.Instance.Damage(CentipedeDefinitions.ContactDamage, transform.position,
                    CentipedeDefinitions.Knockback);
            else Fragment.Damage(Manager.PlayerStats.AttackDamage);
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