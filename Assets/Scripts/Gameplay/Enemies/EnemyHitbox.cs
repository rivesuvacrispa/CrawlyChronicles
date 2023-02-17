using UnityEngine;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        private new Collider2D collider;
        
        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnCollisionEnter2D(Collision2D _)
        {
            enemy.Damage(Player.Movement.Position, Player.Manager.PlayerStats.AttackDamage, Player.Manager.PlayerStats.AttackKnockback);
        }

        /*private void OnParticleCollision(GameObject other)
        {
            // enemy.Damage(other.transform.position, 0, 3);
        }*/

        public void Enable() => collider.enabled = true;
        public void Disable() => collider.enabled = false;
    }
}