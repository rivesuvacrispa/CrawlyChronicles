using System.Collections.Generic;
using System.Linq;
using Gameplay.Effects.Fly;
using Gameplay.Player;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class CorpseScent : BasicAbility
    {
        // TODO: Ability description & icon
        [Header("Damage")] 
        [SerializeField, Range(0.1f, 5f)] private float damageLvl1;
        [SerializeField, Range(0.1f, 5f)] private float damageLvl10;
        [Header("Damage")] 
        [SerializeField, Range(0.25f, 1f)] private float attackCooldownLvl1;
        [SerializeField, Range(0.25f, 1f)] private float attackCooldownLvl10;
        [Header("Fly Speed")] 
        [SerializeField, Range(5, 20)] private float flySpeedLvl1;
        [SerializeField, Range(5, 20)] private float flySpeedLvl10;
        [Header("Rotation Speed")] 
        [SerializeField, Range(20, 100)] private float rotationSpedLvl1;
        [SerializeField, Range(20, 100)] private float rotationSpeedLvl10;
        [Header("Damage")] 
        [SerializeField, Range(1, 20)] private int fliesAmountLvl1;
        [SerializeField, Range(1, 20)] private int fliesAmountLvl10;

        private float damage;
        private float attackCooldown;
        private float flySpeed;
        private float rotationSpeed;
        private int fliesAmount;
        private static readonly List<Fly> Flies = new();


        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            attackCooldown = LerpLevel(attackCooldownLvl1, attackCooldownLvl10, lvl);
            flySpeed = LerpLevel(flySpeedLvl1, flySpeedLvl10, lvl);
            rotationSpeed = LerpLevel(rotationSpedLvl1, rotationSpeedLvl10, lvl);
            fliesAmount = LerpLevel(fliesAmountLvl1, fliesAmountLvl10, lvl);

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
            int currentAmount = Flies.Count;
            int maxAmount = CalculateSummonsAmount(fliesAmount);
            int changeAmount = maxAmount - currentAmount;
            if (changeAmount == 0) return;
            
            if (changeAmount > 0)
                for (int i = 0; i < changeAmount; i++)
                {
                    Fly fly = PoolManager.GetEffect<Fly>(
                        new FlyArguments(damage, attackCooldown, flySpeed, rotationSpeed),
                        position: transform.position + (Vector3)Random.insideUnitCircle * 0.2f);
                    Flies.Add(fly);
                }

            else
            {
                int removeAmount = changeAmount * -1;
                for (int i = 0; i < removeAmount; i++)
                {
                    IPoolable fly = Flies[i];
                    fly.Pool();
                }
                Flies.RemoveRange(0, removeAmount);
            }
        }

        private void ClearFlies()
        {
            foreach (IPoolable fly in Flies) 
                fly.Pool();
            Flies.Clear();
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

        private void OnPlayerStatsChanged()
        {
            SpawnOrDespawnFlies();
        }
    }
}