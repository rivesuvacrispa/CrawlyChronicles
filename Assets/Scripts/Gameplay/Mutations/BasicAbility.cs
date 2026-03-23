using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using Scriptable;
using UI.Elements;
using UI.Menus;
using UnityEngine;
using Util;
using Util.Interfaces;
using Util.Particles;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations
{
    public class BasicAbility : MonoBehaviour
    {
        [SerializeField] protected BasicMutation scriptable;
        [SerializeField, Range(0, 9)] protected int level;

        public BasicMutation Scriptable => scriptable;
        public IAbilityButton Button { get; set; }
        public int Level => level;
        private ParticleCollisionProvider[] collisionProviders;


        protected virtual void Awake()
        {
            collisionProviders = GetComponentsInChildren<ParticleCollisionProvider>();
            foreach (var provider in collisionProviders) 
                provider.OnCollision += OnBulletCollision;
        }

        protected virtual void OnDestroy()
        {
            foreach (var provider in collisionProviders) 
                provider.OnCollision -= OnBulletCollision;
        }

        protected virtual void Start() => SetLevel(level, true);

        protected virtual void OnEnable()
        {
            if (Button is not null)
                Button.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            Button.SetActive(false);
        }

        protected virtual void OnBulletCollision(IDamageable damageable, int collisionID)
        {
            
        }

        protected static bool TryProc(float baseRate)
        {
            float x = PlayerManager.PlayerStats.PassiveProcRate;
            float powBase = 1 / (1 - baseRate) * -1;
            float powAmount = (2 * x + 1) * -1;
            float mapped = Mathf.Pow(powBase, powAmount) + 1;
            return Random.value < mapped;
        }


        
        public void SetLevel(int newLevel, bool forceUpdate = false)
        {
            if(newLevel == level && !forceUpdate) return;
            level = Mathf.Clamp(newLevel, 0, 9);
            Button.UpdateLevelText(level);
            OnLevelChanged(level);
        }

        public virtual void OnLevelChanged(int lvl)
        {
            if(Button is not null) Button.UpdateLevelText(lvl);
        }

        protected CancellationToken CreateCommonCancellationToken(params CancellationToken[] tokens)
        {
            return gameObject.CreateCommonCancellationToken(tokens);
        }

        protected static float LerpLevel(float from, float to, int lvl) => Mathf.Lerp(from, to, lvl / 9f);

        protected static int LerpLevel(int from, int to, int lvl) => Mathf.RoundToInt(Mathf.Lerp(from, to, lvl / 9f));

        public virtual string GetLevelDescription(int lvl, bool withUpgrade) => string.Empty;

        public static float CalculateParticleSize(float baseSize) =>
            baseSize * (1 * PlayerManager.PlayerStats.ProjectileSize);
        
        public static float CalculateParticleAmount(float baseAmount) =>
            baseAmount * (1 * PlayerManager.PlayerStats.ProjectileAmount);
        
        // TODO: apply to every ability that should benify from it
        public static float CalculateAbilityDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.AbilityDamage);

        public static float CalculateSummonDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.SummonDamage);
        
        public static int CalculateSummonsAmount(int baseAmount) =>
            baseAmount + PlayerManager.PlayerStats.BonusSummonAmount;
    }
}