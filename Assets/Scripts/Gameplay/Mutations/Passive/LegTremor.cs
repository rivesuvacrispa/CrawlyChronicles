using System;
using System.Collections.Generic;
using Gameplay.Effects.LilHorror;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Passive
{
    public class LegTremor : BasicAbility
    {
        [SerializeField, MinMaxRange(0.1f, 10f)] private LevelFloat contactDamage = new LevelFloat(1, 2.5f);
        [SerializeField, MinMaxRange(0.01f, 10f)] private LevelFloat speed = new LevelFloat(2, 4);
        [SerializeField, MinMaxRange(0.1f, 360f)] private LevelFloat rotationSpeed = new LevelFloat(160, 260);


        public static float CurrentContactDamage { get; private set; }
        public static float CurrentMoveSpeed { get; private set; }
        public static float CurrentRotationSpeed { get; private set; }
        private int bodyLength;
        
        private LilHorrorPart headPart;
        private readonly List<LilHorrorPart> parts = new();


        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            if (lvl == 0) return Array.Empty<ILevelField>();
            
            return new[]
            {
                contactDamage.UseKey(LevelFieldKeys.CONTACT_DAMAGE),
                speed.UseKey(LevelFieldKeys.MOVEMENT_SPEED),
                rotationSpeed.UseKey(LevelFieldKeys.ROTATION_SPEED)
            };
        }
        
        protected override bool CacheLevelFields => false;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            CurrentContactDamage = contactDamage.AtLvl(lvl);
            CurrentMoveSpeed = speed.AtLvl(lvl);
            CurrentRotationSpeed = rotationSpeed.AtLvl(lvl);
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