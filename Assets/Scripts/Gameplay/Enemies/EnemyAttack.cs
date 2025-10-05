using System;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    public class EnemyAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private Enemy enemy;

        
        private void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log($"ATTACK Collided, GO: {col.gameObject.name}");

            if (col.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                // OnStruck?.Invoke(0);
                ((IDamageable)playerManager)
                    .Damage(AttackDamage, AttackPosition, 
                        AttackPower, 0, default);
            }
        }


        public Vector3 AttackPosition => enemy.transform.position;
        public float AttackDamage => enemy.Scriptable.Damage;
        public float AttackPower => enemy.Scriptable.AttackPower;
    }
}