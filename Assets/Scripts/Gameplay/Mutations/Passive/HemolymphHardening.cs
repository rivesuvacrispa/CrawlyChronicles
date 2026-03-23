using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;
using Util.Interfaces;
using Util.Particles;

namespace Gameplay.Mutations.Passive
{
    public class HemolymphHardening : BasicAbility, IDamageSource
    {
        [Header("References")]
        [SerializeField] private BulletParticleSystem burst;
        [SerializeField] private ParticleSystem orbits;

        [Header("Health per blob")]
        [SerializeField] private float healthPerBlob;

        [Header("Max Blobs")]
        [SerializeField, Range(0, 20)] private int maxBlobsLvl1;

        [SerializeField, Range(0, 20)] private int maxBlobsLvl10;

        [Header("Max Blobs")]
        [SerializeField, Range(0f, 10f)] private float damageLvl1;

        [SerializeField, Range(0f, 10f)] private float damageLvl10;

        [Header("Explosion blob amount")]
        [SerializeField, Range(0f, 50f)] private float explosionBlobAmountLvl1;

        [SerializeField, Range(0f, 50f)] private float explosionBlobAmountLvl10;

        private float damage;
        private int maxBlobs;
        private int currentBlobsAmount = 0;
        private float accumulatedHealth = 0;


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            maxBlobs = LerpLevel(maxBlobsLvl1, maxBlobsLvl10, lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            
            burst.SetBaseAmount(LerpLevel(explosionBlobAmountLvl1, explosionBlobAmountLvl10, lvl));
            // ParticleSystem.EmissionModule e = burst.emission;
            // var b = e.GetBurst(0);
            // float blobAmount =;
            // b.count = new ParticleSystem.MinMaxCurve(blobAmount * 0.5f, blobAmount);
            // e.SetBurst(0, b);
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
            if (currentBlobsAmount >= maxBlobs) return;

            accumulatedHealth += amount;
            int blobsFormed = Mathf.FloorToInt(accumulatedHealth / healthPerBlob);
            
            if (blobsFormed == 0) return;
            accumulatedHealth %= blobsFormed;
            currentBlobsAmount += blobsFormed;
            
            print($"Added HP: {amount}, blobs formed: {blobsFormed}, accumulated: {accumulatedHealth}, current blobs: {currentBlobsAmount}");

            if (currentBlobsAmount >= maxBlobs)
                accumulatedHealth = 0;

            UpdateBlobs();
        }

        protected override void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            if (damageable is IDamageableEnemy enemy)
                enemy.Damage(
                    new DamageSource(this, collisionID),
                    CalculateAbilityDamage(damage),
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