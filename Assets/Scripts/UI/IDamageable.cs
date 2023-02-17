using UnityEngine;

namespace UI
{
    public interface IDamageable
    {
        public delegate void DamageableEvent();
        public event DamageableEvent OnDamageableDestroy;
        public Transform Transform { get; }
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
    }
}