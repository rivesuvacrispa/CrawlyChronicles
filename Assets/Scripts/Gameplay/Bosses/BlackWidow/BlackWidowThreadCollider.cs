using Gameplay.Bosses.Centipede;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowThreadCollider : MonoBehaviour
    {
        [SerializeField] private BlackWidowThread thread;
        [SerializeField] private bool immune;

        private bool dead;
        
        
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            Vector3 contactPoint = thread.FindClosestPointOnLine(col.transform.position);
            
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
            {
                PlayerManager.Instance.Damage(CentipedeBoss.ContactDamage, contactPoint,
                    CentipedeDefinitions.Knockback);
                
            } else if (!(dead || immune))
            {
                dead = true;
                thread.DieOnHit(contactPoint).Forget();
            }
        }
    }
}