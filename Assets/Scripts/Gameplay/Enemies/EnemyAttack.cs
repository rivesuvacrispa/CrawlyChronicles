using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    public class EnemyAttack : MonoBehaviour, IEnemyAttack, IDamageSource
    {
        [SerializeField] private Enemy enemy;

        /***
         * EnemyAttack x PlayerHitbox
         * Enemy attack collides with player hitbox
         * Player takes damage
         */
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                ((IDamageable)playerManager)
                    .Damage(new DamageInstance(
                        new DamageSource(this, col.GetHashCode()),
                        AttackDamage,
                        AttackPosition,
                        AttackPower));
            }
        }


        public Vector3 AttackPosition => enemy.transform.position;
        public float AttackDamage => enemy.Scriptable.Damage;
        public float AttackPower => enemy.Scriptable.AttackPower;
    }
}