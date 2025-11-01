using System;
using System.Collections.Generic;
using Gameplay.Player;
using Hitboxes;
using Scriptable;
using UI.Elements;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Mutations
{
    public class BasicAbility : MonoBehaviour
    {
        [SerializeField] protected BasicMutation scriptable;
        [SerializeField, Range(0, 9)] protected int level;

        public BasicMutation Scriptable => scriptable;
        public BasicAbilityButton Button { get; set; }
        public int Level => level;
        private ParticleCollisionProvider[] collisionProviders;


        protected virtual void Awake()
        {
            collisionProviders = GetComponentsInChildren<ParticleCollisionProvider>();
            foreach (var provider in collisionProviders) 
                provider.OnCollision += OnBulletCollision;
        }

        private void OnDestroy()
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

        protected static float GetPassiveProcRate(float proc) => proc * PlayerManager.PlayerStats.PassiveProcRate;

        public static float GetAbilityDamage(float damage) => damage + damage * PlayerManager.PlayerStats.AbilityDamage;
        
        public void SetLevel(int newLevel, bool forceUpdate = false)
        {
            if(newLevel == level && !forceUpdate) return;
            level = Mathf.Clamp(newLevel, 0, 9);
            Button.UpdateLevelText(level);
            OnLevelChanged(level);
        }

        protected static float LerpLevel(float from, float to, int lvl) => Mathf.Lerp(from, to, lvl / 9f);

        protected static int LerpLevel(int from, int to, int lvl) => Mathf.RoundToInt(Mathf.Lerp(from, to, lvl / 9f));

        public virtual void OnLevelChanged(int lvl)
        {
            if(Button is not null) Button.UpdateLevelText(lvl);
        }

        public virtual string GetLevelDescription(int lvl, bool withUpgrade) => string.Empty;
    }
}