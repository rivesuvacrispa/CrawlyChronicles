using UnityEngine;

namespace UI
{
    public interface IDamageable
    {
        public Transform Transform { get; }
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
    }
}