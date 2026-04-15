using System.Collections.Generic;
using System.Linq;
using Gameplay.Effects.Fly;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Passive
{
    public class CorpseScent : BasicAbility
    {
        [SerializeField, MinMaxRange(0.1f, 5f)] private LevelFloat damage = new LevelFloat(0.5f, 1.5f);
        [SerializeField, MinMaxRange(0.25f, 1f)] private LevelFloat attackCooldown = new LevelFloat(1, 0.5f);
        [SerializeField, MinMaxRange(5, 20)] private LevelFloat flySpeed = new LevelFloat(8, 16);
        [SerializeField, MinMaxRange(20, 100)] private LevelFloat rotationSpeed = new LevelFloat(20, 80);
        [SerializeField, MinMaxRange(1, 20)] private LevelInt fliesAmount = new LevelInt(1, 10);

        private float currentDamage;
        private float currentAttackCooldown;
        private float currentFlySpeed;
        private float currentRotationSpeed;
        private int currentFliesAmount;
        private readonly List<Fly> flies = new();
        
        

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDamage = damage.AtLvl(lvl);
            currentAttackCooldown = attackCooldown.AtLvl(lvl);
            currentFlySpeed = flySpeed.AtLvl(lvl);
            currentRotationSpeed = rotationSpeed.AtLvl(lvl);
            currentFliesAmount = fliesAmount.AtLvl(lvl);

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (isActiveAndEnabled)
                {
                    SpawnOrDespawnFlies();
                }
#if UNITY_EDITOR
            }
#endif
        }
        
        private void SpawnOrDespawnFlies()
        {
            int currentAmount = flies.Count;
            int maxAmount = CalculateSummonsAmount(currentFliesAmount);
            int changeAmount = maxAmount - currentAmount;
            if (changeAmount == 0) return;
            
            if (changeAmount > 0)
                for (int i = 0; i < changeAmount; i++)
                {
                    Fly fly = PoolManager.GetEffect<Fly>(
                        new FlyArguments(currentDamage, currentAttackCooldown, currentFlySpeed, currentRotationSpeed),
                        position: transform.position + (Vector3)Random.insideUnitCircle * 0.2f);
                    flies.Add(fly);
                }

            else
            {
                int removeAmount = changeAmount * -1;
                for (int i = 0; i < removeAmount; i++)
                {
                    IPoolable fly = flies[i];
                    fly.Pool();
                }
                flies.RemoveRange(0, removeAmount);
            }
        }

        private void ClearFlies()
        {
            foreach (IPoolable fly in flies) 
                fly.Pool();
            flies.Clear();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SpawnOrDespawnFlies();
            PlayerManager.OnStatsChanged += OnPlayerStatsChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearFlies();
            PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;
        }

        private void OnPlayerStatsChanged(PlayerStats changes)
        {
            if (changes.BonusSummonAmount != 0)
                SpawnOrDespawnFlies();
        }
        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                damage.UseKey(LevelFieldKeys.FLIES_DAMAGE),
                attackCooldown.UseKey(LevelFieldKeys.FLIES_ATTACK_INTERVAL),
                flySpeed.UseKey(LevelFieldKeys.FLIES_SPEED),
                rotationSpeed.UseKey(LevelFieldKeys.FLIES_ROTATION_SPEED),
                fliesAmount.UseKey(LevelFieldKeys.FLIES_AMOUNT),
            };
        }
    }
}