using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Effects.LilHorror;
using Gameplay.Player;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class LegTremor : BasicAbility
    {
        [Header("Contact Damage")] 
        [SerializeField, Range(0.1f, 10f)] private float contactDamageLvl1;
        [SerializeField, Range(0.1f, 10f)] private float contactDamageLvl10;
        [Header("Movespeed")] 
        [SerializeField, Range(0.001f, 10f)] private float speedLvl1;
        [SerializeField, Range(0.001f, 10f)] private float speedLvl10;
        [Header("Rotation speed")] 
        [SerializeField, Range(0.1f, 360f)] private float rotationSpeedLvl1;
        [SerializeField, Range(0.1f, 360f)] private float rotationSpeedLvl10;

        public static float ContactDamage { get; private set; }
        public static float MoveSpeed { get; private set; }
        public static float RotationSpeed { get; private set; }
        private int bodyLength;
        
        private LilHorrorPart headPart;
        private readonly List<LilHorrorPart> parts = new();
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            ContactDamage = LerpLevel(contactDamageLvl1, contactDamageLvl10, lvl);
            MoveSpeed = LerpLevel(speedLvl1, speedLvl10, lvl);
            RotationSpeed = LerpLevel(rotationSpeedLvl1, rotationSpeedLvl10, lvl);
            bodyLength = lvl + 2;

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (isActiveAndEnabled)
                {
                    AdjustBodyLength();
                }
#if UNITY_EDITOR
            }
#endif
        }

        private void AdjustBodyLength()
        {
            int currentAmount = parts.Count;
            int maxAmount = CalculateSummonsAmount(bodyLength);
            int changeAmount = maxAmount - currentAmount;
            Debug.Log($"Leg tremor change, current: {currentAmount}, max: {maxAmount}, change: {changeAmount}");
            if (changeAmount == 0) return;
            
            if (headPart is null) 
                CreateHeadPart();

            
            if (changeAmount > 0)
            {
                LilHorrorPart currentPart = GetTailPart();
                for (int i = 0; i <= changeAmount; i++)
                {
                    currentPart = CreatePart(currentPart);
                    parts.Add(currentPart);
                }
            }
            else
            {
                int removeAmount = changeAmount * -1;
                for (int i = 0; i < removeAmount; i++)
                {
                    var part = GetTailPart();
                    if (part is not null)
                    {
                        parts.Remove(part);
                        ((IPoolable)part).Pool();
                    }
                }
            }
            
            Debug.Log($"Leg tremor final count: {parts.Count}");
        }

        private void ClearParts()
        {
            foreach (LilHorrorPart part in parts) 
                ((IPoolable)part).Pool();

            parts.Clear();
            if (headPart is not null)
            {
                ((IPoolable)headPart).Pool();
                headPart = null;
            }
        }

        private LilHorrorPart GetTailPart()
        {
            return parts.Count == 0 ? headPart : parts[^1];
        }

        private LilHorrorPart CreatePart(LilHorrorPart parent)
        {
            Vector3 pos = GetTailPart()?.transform.position ?? PlayerPhysicsBody.Position;
            return PoolManager.GetEffect<LilHorrorPart>(new LilHorrorPartArguments(parent), pos);
        }

        private void CreateHeadPart()
        {
            headPart = CreatePart(null);
            headPart.gameObject.AddComponent<LilHorrorHead>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            AdjustBodyLength();
            PlayerManager.OnStatsChanged += OnPlayerStatsChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearParts();
            PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;
        }

        private void OnPlayerStatsChanged(PlayerStats changes)
        {
            if (changes.BonusSummonAmount != 0)
                AdjustBodyLength();
        }
    }
}