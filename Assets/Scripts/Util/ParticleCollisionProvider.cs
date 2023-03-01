using UI;
using UnityEngine;

namespace Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCollisionProvider : MonoBehaviour
    {
        public delegate void CollisionEvent(IDamageable enemy);
        public event CollisionEvent OnCollision;
        
        
        
        private void OnParticleCollision(GameObject other)
        {
            if (other.TryGetComponent(out IDamageable damageable)) 
                OnCollision?.Invoke(damageable);
        }
    }
}