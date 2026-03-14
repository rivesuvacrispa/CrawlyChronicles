using UnityEngine;

namespace Gameplay.Effects.PhantomPlayerAttack
{
    public readonly struct PhantomPlayerAttackArguments
    {
        public readonly Vector3 spawnPos;
        public readonly Vector3 targetPos;
        public readonly float lifetime;
        public readonly float bonusDamage;

        public PhantomPlayerAttackArguments(Vector3 spawnPos, Vector3 targetPos, float bonusDamage = 0f, float lifetime = 0.4f)
        {
            this.spawnPos = spawnPos;
            this.targetPos = targetPos;
            this.bonusDamage = bonusDamage;
            this.lifetime = lifetime;
        }
    }
}