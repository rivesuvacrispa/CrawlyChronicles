using Gameplay.Effects.BroodSpider;
using Hitboxes;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.EntityEffects.BroodInfection
{
    public class BroodInfectionEffect : EntityEffect
    {
        protected override void OnApplied()
        {
            if (Target is IDamageableEnemy enemy)
                enemy.OnDeath += OnTargetDeath;
        }

        protected override void Tick()
        {
        }

        protected override void OnRemoved()
        {
            if (Target is IDamageableEnemy enemy)
                enemy.OnDeath -= OnTargetDeath;
        }

        private void OnTargetDeath(IDamageable damageable)
        {
            if (!BroodSpider.CanSpawnSpider)
                return;
            
            BroodInfectionEffectData data = (BroodInfectionEffectData) Data;

            float spiderAmount = data.BaseSpiderAmount;
            float spawnChance = data.BaseSpawnChance;
            Debug.Log($"Spawn chance: {spawnChance}, spawn amount: {spiderAmount}");
            int full = Mathf.FloorToInt(spiderAmount);
            float left = spiderAmount - full;
            Vector3 pos = Target.Transform.position;
            
            for (int i = 0; i < full; i++)
            {
                if (Random.value < spawnChance)
                    SpawnSpider(data.SpiderArguments, pos);
            }
            
            if (left > 0 && Random.value < spawnChance * left)
                SpawnSpider(data.SpiderArguments, pos);
        }

        private void SpawnSpider(BroodSpiderArguments args, Vector3 pos) => PoolManager.GetEffect<BroodSpider>(args, pos);
    }
}