using System;
using Hitboxes;
using Pooling;
using UnityEngine;

namespace Util.Particles
{
    public class ParticleTrigger : Poolable
    {
        [SerializeField] private CircleCollider2D col;

        public void SetActive(bool state) => col.enabled = state;
        private int triggerId;
        
        public delegate void TriggerEvent(IDamageable col, int triggerId);
        public event TriggerEvent OnTrigger;
        
        
        
        private void Awake()
        {
            triggerId = GetHashCode();
            col.isTrigger = true;
        }

        public override bool OnTakenFromPool(object data)
        {
            if (data is not float f) return false;

            col.radius = f;
            
            return base.OnTakenFromPool(data);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out DamageableHitbox hitbox))
            {
                OnTrigger?.Invoke(hitbox.Damageable, triggerId);
            }
        }
    }
}