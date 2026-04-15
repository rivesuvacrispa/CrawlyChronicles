using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Abilities;
using Util.Attributes;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Passive
{
    public class HemolymphHardening : BasicAbility, IDamageSource
    {
        [Header("References")]
        [SerializeField] private BulletParticleSystem burst;
        [SerializeField] private ParticleSystem orbits;
        [Header("Stats")]
        [SerializeField] private LevelConst healthPerBlob = new LevelConst(3);
        [SerializeField, MinMaxRange(0, 20)] private LevelInt maxBlobs = new LevelInt(2, 10);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat damage = new LevelFloat(0.5f, 2f);
        [SerializeField, MinMaxRange(0f, 50f)] private LevelFloat explosionBlobAmount = new LevelFloat(5f, 15f);

        private float currentDamage;
        private int currentMaxBlobs;
        private int currentBlobsAmount = 0;
        private float accumulatedHealth = 0;


        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                healthPerBlob.UseKey(LevelFieldKeys.HEALTH_PER_BLOB),
                maxBlobs.UseKey(LevelFieldKeys.MAX_BLOBS),
                damage.UseKey(LevelFieldKeys.DAMAGE),
                explosionBlobAmount.UseKey(LevelFieldKeys.EXPLOSION_PARTICLES_AMOUNT)
            };
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentMaxBlobs = maxBlobs.AtLvl(lvl);
            currentDamage = damage.AtLvl(lvl);
            burst.SetBaseAmount(explosionBlobAmount.AtLvl(lvl));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerManager.OnBeforeHealthAdded += OnHealthAdded;
            PlayerManager.OnTryBlockDamage += OnTryBlockDamage;
            UpdateBlobs();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerManager.OnBeforeHealthAdded -= OnHealthAdded;
            PlayerManager.OnTryBlockDamage -= OnTryBlockDamage;
            currentBlobsAmount = 0;
            accumulatedHealth = 0;
        }

        private void OnTryBlockDamage(MultiSourceState blockState)
        {
            if (currentBlobsAmount <= 0) return;

            blockState.Vote(GetHashCode());
            burst.Particles.Play();
            currentBlobsAmount--;
            UpdateBlobs();
        }

        private void OnHealthAdded(float amount)
        {
            if (currentBlobsAmount >= currentMaxBlobs) return;

            accumulatedHealth += amount;
            int blobsFormed = Mathf.FloorToInt(accumulatedHealth / healthPerBlob.Value);
            
            if (blobsFormed == 0) return;
            accumulatedHealth %= blobsFormed;
            currentBlobsAmount += blobsFormed;
            
            print($"Added HP: {amount}, blobs formed: {blobsFormed}, accumulated: {accumulatedHealth}, current blobs: {currentBlobsAmount}");

            if (currentBlobsAmount >= currentMaxBlobs)
                accumulatedHealth = 0;

            UpdateBlobs();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is IDamageableEnemy enemy)
                enemy.Damage(
                    new DamageSource(this, collisionID),
                    CalculateAbilityDamage(currentDamage),
                    transform.position, piercing: true
                );
        }

        private void UpdateBlobs()
        {
            orbits.Clear();
            orbits.Emit(new ParticleSystem.EmitParams(), currentBlobsAmount);
        }
    }
}