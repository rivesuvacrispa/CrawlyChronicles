using System.Collections.Generic;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Util
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCollisionProvider : MonoBehaviour
    {
        private readonly List<ParticleCollisionEvent> collisionEvents = new();
        private new ParticleSystem particleSystem;
        
        public delegate void CollisionEvent(IDamageable col, int collisionID);
        public event CollisionEvent OnCollision;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            int cols = particleSystem.GetCollisionEvents(other, collisionEvents);
            
            if (other.TryGetComponent(out IDamageable damageable))
            {
                OnCollision?.Invoke(damageable, cols == 0 ? 0 : collisionEvents[0].GetHashCode());
            }
        }
    }
}