using System.Collections;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Bosses.Centipede
{
    public class CentipedeAttack : MonoBehaviour, IDamageSource
    {
        private new Collider2D collider;

        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (((IDamageable)PlayerManager.Instance).Damage(new DamageInstance(
                    new DamageSource(this),
                    CentipedeDefinitions.AttackDamage,
                    transform.position,
                    CentipedeDefinitions.Knockback)) == 0)
            {
                PlayerManager.Instance.Knockback((Vector2) transform.position + Random.insideUnitCircle.normalized,
                    CentipedeDefinitions.Knockback);
            }
            if(gameObject.activeInHierarchy) StartCoroutine(DelayRoutine());
        }

        private IEnumerator DelayRoutine()
        {
            collider.enabled = false;
            yield return new WaitForSeconds(5f);
            collider.enabled = true;
        }
        
    }
}