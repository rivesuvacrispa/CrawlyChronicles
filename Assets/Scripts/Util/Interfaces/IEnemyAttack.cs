using UnityEngine;

namespace Util.Interfaces
{
    public interface IEnemyAttack
    {
        public Vector3 AttackPosition { get; }
        public float AttackDamage { get; }
        public float AttackPower { get; }
    }
}