using Gameplay.Bosses.Centipede;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowThreadCollider : MonoBehaviour, IDamageSource
    {
        [SerializeField] private BlackWidowThread thread;
        [SerializeField] private bool immune;

        private bool dead;
        
        
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            Vector3 contactPoint = thread.FindClosestPointOnLine(col.transform.position);
            
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
            {
                ((IDamageable)PlayerManager.Instance).Damage(new DamageInstance(
                    new DamageSource(this),
                    CentipedeBoss.ContactDamage, contactPoint,
                    CentipedeDefinitions.Knockback));
                
            } else if (!(dead || immune))
            {
                dead = true;
                thread.DieOnHit(contactPoint).Forget();
            }
        }
    }
}