using System.Collections;
using Gameplay.Enemies;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private float knockback;
        [SerializeField] private Movement movement;
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private Gradient immunityGradient;
        
        private new Collider2D collider;
        
        public bool BlockColor { get; set; }

        public delegate void PlayerHitboxEvent(float damage);
        public static event PlayerHitboxEvent OnDamageTaken;
        
        
        
        private void Awake() => collider = GetComponent<Collider2D>();

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(col.gameObject.TryGetComponent(out Enemy enemy))
                Damage(col.transform.position, enemy.Scriptable.Damage, enemy.Scriptable.Knockback);
        }
        
        private void Damage(Vector3 pos, float damage, float kb)
        {
            if (Manager.Instance.GodMode) return;
            movement.Knockback(pos, 0.35f, kb);
            Manager.Instance.Damage(damage);
            OnDamageTaken?.Invoke(damage);
            StartCoroutine(ImmunityRoutine());
        }
        
        private IEnumerator ImmunityRoutine()
        {
            Disable();

            float duration = Manager.PlayerStats.ImmunityDuration;
            float t = 0;
            while (t < duration)
            {
                if(!BlockColor) bodySprite.color = immunityGradient.Evaluate(t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            bodySprite.color = immunityGradient.Evaluate(1);
            Enable();
        }
        
        public void Enable() => collider.enabled = true;
        public void Disable() => collider.enabled = false;
    }
}