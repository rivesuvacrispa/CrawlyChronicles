using Definitions;
using Gameplay.Enemies;
using Scripts.SoundEffects;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!AttackController.IsInComboDash &&
                col.collider.gameObject.layer.Equals(GlobalDefinitions.EnemyAttackLayerMask) && 
                col.gameObject.TryGetComponent(out Enemy enemy))
            {
                PlayerAudioController.Instance.PlayReckoning();
                Vector2 point = col.contacts[0].point;
                float force = Manager.PlayerStats.AttackPower;
                movement.Knockback(point, force);
                enemy.Reckon(point, force);
            }
        }
    }
}