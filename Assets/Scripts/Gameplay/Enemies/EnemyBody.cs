using UnityEngine;

namespace Gameplay.Enemies
{
    public class EnemyBody : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        
        
        private void OnCollisionEnter2D(Collision2D _)
        {
            enemy.Damage(Player.Movement.Position, Player.AttackController.AttackDamage, Player.AttackController.KnockbackPower);
        }
    }
}