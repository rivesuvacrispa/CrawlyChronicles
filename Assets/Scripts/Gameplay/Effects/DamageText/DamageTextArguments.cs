using UnityEngine;

namespace Gameplay.Effects.DamageText
{
    public readonly struct DamageTextArguments
    {
        public readonly float damage;
        public readonly Vector3 position;

        public DamageTextArguments(Vector3 position, float damage)
        {
            this.position = position;
            this.damage = damage;
        }
    }
}