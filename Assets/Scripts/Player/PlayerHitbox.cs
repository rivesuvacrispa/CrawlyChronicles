using System.Collections;
using Gameplay.Interfaces;
using UnityEngine;
using Util;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private Gradient immunityGradient;
        
        public bool Immune { get; private set; }
        
        public bool BlockColorChange { get; set; }

        public delegate void PlayerHitboxEvent(float damage);
        public static event PlayerHitboxEvent OnStruck;
        
        
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent(out IEnemyAttack attack))
            {
                OnStruck?.Invoke(0);
                PlayerManager.Instance.Damage(attack.AttackDamage, attack.AttackPosition, attack.AttackPower);
            }
        }

        public void Hit() => StartCoroutine(ImmunityRoutine());

        private IEnumerator ImmunityRoutine()
        {
            Immune = true;

            float duration = PlayerManager.PlayerStats.ImmunityDuration;
            bodyPainter.Paint(immunityGradient, duration);
            yield return new WaitForSeconds(duration);
            
            Immune = false;
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            Immune = false;
        }

        public void Disable()
        {
            StopAllCoroutines();
            Immune = true;
        }
    }
}