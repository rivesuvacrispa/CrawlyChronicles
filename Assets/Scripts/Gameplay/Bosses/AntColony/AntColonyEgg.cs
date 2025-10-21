using System.Collections;
using Definitions;
using Gameplay.Enemies;
using Gameplay.Map;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Bosses.AntColony
{
    public class AntColonyEgg : MonoBehaviour, IDamageableEnemy
    {
        [SerializeField] private DamageableEnemyHitbox hitbox;
        
        public Rigidbody2D Rb { get; private set; }
        public Enemy ToHatch { get; set; }

        private float maxHealth;
        

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            maxHealth = AntColonyDefinitions.EggsHealth;
            CurrentHealth = maxHealth;
        }

        private void Start()
        {
            StartCoroutine(HatchRoutine());
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private IEnumerator HatchRoutine()
        {
            yield return new WaitForSeconds(AntColonyDefinitions.EggsHatchTime *
                                            Random.Range(0.95f, 1.05f));
            var enemy = Instantiate(ToHatch, MapManager.GameObjectsTransform);
            enemy.OnSpawnedBySpawner();
            enemy.transform.position = transform.position;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            OnDeath?.Invoke(this);
            GlobalDefinitions.CreateEggSquash(transform.position);
            OnProviderDestroy?.Invoke(this);
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        private void OnResetRequested() => Destroy(gameObject);


        // IDamageable
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public float HealthbarOffsetY => -0.25f;
        public float HealthbarWidth => 80f;
        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public bool Immune => hitbox.Immune;
        public float Armor => 0;
        public float CurrentHealth { get; set; }
        public float MaxHealth => AntColonyDefinitions.EggsHealth;

        public void OnBeforeHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            OnDamageTaken?.Invoke(this, damage);
        }

        public void OnLethalHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            Destroy(gameObject);
            hitbox.Die();
        }

        public void OnHit(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool piercing = false)
        {
            hitbox.Hit();
        }
    }
}