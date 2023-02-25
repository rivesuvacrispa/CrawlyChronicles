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
            enemy.Damage(Player.Manager.PlayerStats.AttackDamage, 
                Player.Manager.PlayerStats.AttackPower, 0.5f);
        }

        public void Enable() => collider.enabled = true;

        public void Disable() => collider.enabled = false;

        public bool Enabled => collider.enabled;
    }
}