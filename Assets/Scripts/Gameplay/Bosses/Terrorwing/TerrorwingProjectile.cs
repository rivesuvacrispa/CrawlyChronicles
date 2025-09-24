using Gameplay.Player;
using UI;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Bosses.Terrorwing
{
    public class TerrorwingProjectile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem explosion;
        [SerializeField] private Animator animator;
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;

        private float lifeTime;
        private static readonly int ExplosionAnimHash = Animator.StringToHash("TerrorwingProjectileExplosion");
        public Vector2 Target { get; set; } = Vector2.zero;

        // Used by animator
        public void Destroy() => Destroy(gameObject);

        private void Explode()
        {
            enabled = false;
            animator.enabled = true;
            explosion.Play();
            animator.Play(ExplosionAnimHash);
        }

        private void Start()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            if (Target.Equals(Vector2.zero)) Target = PlayerMovement.Position;
            Vector2 pos = transform.position;
            RotateTowardsTarget(Target - pos, 360);
        }

        private void Update()
        {
            if(lifeTime >= 5f)
            {
                Explode();
                return;
            }
            Vector2 pos = transform.position;
            Vector2 direction = pos - Target;

            float distance = direction.sqrMagnitude;
            if (distance < 0.25f)
            {
                Explode();
                return;
            }

            float delta = Time.deltaTime;
            transform.position = Vector2.MoveTowards(pos, pos + (Vector2) transform.up, speed * delta);
            lifeTime += delta;
            RotateTowardsTarget(direction, rotationSpeed);
        }

        private void RotateTowardsTarget(Vector2 direction, float delta)
        {
            float a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0, a));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, delta * Time.deltaTime);
        }

        private void OnResetRequested() => Destroy(gameObject);

        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.TryGetComponent(out PlayerHitbox _))
            {
                Explode();
                PlayerManager.Instance.Damage(
                    TerrorwingDefinitions.ExplosionDamage,
                    transform.position + (Vector3) Random.insideUnitCircle.normalized,
                    5);
            }
        }
    }
}