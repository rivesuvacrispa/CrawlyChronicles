using UnityEngine;

namespace Scripts.Gameplay.Bosses.Terrorwing
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
            animator.enabled = true;
            explosion.Play();
            animator.Play(ExplosionAnimHash);
        }

        private void Start()
        {
            if (Target.Equals(Vector2.zero)) Target = Player.Movement.Position;
            Vector2 pos = transform.position;
            RotateTowardsTarget(Target - pos, 360);
        }

        private void Update()
        {
            if(lifeTime >= 15f)
            {
                Explode();
                enabled = false;
                return;
            }
            Vector2 pos = transform.position;
            Vector2 direction = pos - Target;

            float distance = direction.sqrMagnitude;
            if (distance < 0.25f)
            {
                Explode();
                enabled = false;
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
    }
}