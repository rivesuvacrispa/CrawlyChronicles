using System;
using System.Text;
using System.Threading;
using Gameplay.Player;
using Hitboxes;
using Scriptable;
using UI.Elements;
using UnityEngine;
using UnityEngine.Localization.SmartFormat;
using Util;
using Util.Abilities;
using Util.Particles;
using Random = UnityEngine.Random;

namespace Gameplay.Mutations
{
    public abstract class BasicAbility : MonoBehaviour
    {
        [SerializeField] protected BasicMutation scriptable;
        [SerializeField, Range(0, 9)] protected int level;

        public BasicMutation Scriptable => scriptable;
        public IAbilityButton Button { get; set; }
        public int Level => scriptable.NotUpgradeable ? 9 : level;
        private ParticleCollisionProvider[] collisionProviders;
        private ParticleTriggerProvider[] triggerProviders;
        private static readonly SmartFormatter Formatter = Smart.CreateDefaultSmartFormat();
        private ILevelField[] levelFields;


        protected virtual void Awake()
        {
            collisionProviders = GetComponentsInChildren<ParticleCollisionProvider>();
            foreach (var provider in collisionProviders)
                provider.OnCollision += OnBulletCollision;

            triggerProviders = GetComponentsInChildren<ParticleTriggerProvider>();
            foreach (var provider in triggerProviders)
                provider.OnTrigger += OnBulletCollision;
        }

        protected virtual void OnDestroy()
        {
            foreach (var provider in collisionProviders)
                provider.OnCollision -= OnBulletCollision;

            foreach (var provider in triggerProviders)
                provider.OnTrigger -= OnBulletCollision;
        }

        protected virtual void Start() => SetLevel(Level, true);

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
            if (newLevel == Level && !forceUpdate) return;
            level = Mathf.Clamp(newLevel, 0, 9);
            Button.UpdateLevelText(Level);
            OnLevelChanged(Level);
        }

        public virtual void OnLevelChanged(int lvl)
        {
            if (Button is not null) Button.UpdateLevelText(lvl);
        }

        protected CancellationToken CreateCommonCancellationToken(params CancellationToken[] tokens)
        {
            return gameObject.CreateCommonCancellationToken(tokens);
        }

        public static float LerpLevel(float from, float to, int lvl)
        {
            return Mathf.Lerp(from, to, Mathf.Clamp(lvl, 0, 9) / 9f);
        }

        public static int LerpLevel(int from, int to, int lvl)
        {
            return Mathf.RoundToInt(Mathf.Lerp(from, to, Mathf.Clamp(lvl, 0, 9) / 9f));
        }

        public static int LerpLevelFloor(int from, int to, int lvl)
        {
            return Mathf.FloorToInt(Mathf.Lerp(from, to, Mathf.Clamp(lvl, 0, 9) / 9f));
        }
        
        public static float CalculateAbilityDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.AbilityDamage);

        public static float CalculateSummonDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.SummonDamage);

        public static int CalculateSummonsAmount(int baseAmount) =>
            baseAmount + PlayerManager.PlayerStats.BonusSummonAmount;

        protected abstract ILevelField[] CreateLevelFields(int lvl);

        protected virtual bool CacheLevelFields => true;

        private static object[] BuildDescriptionArguments(
            int lvl, bool withUpgrade,
            params ILevelField[] fields)
        {
            int argsLen = fields.Length;
            object[] o = new object[argsLen * 2];

            for (var index = 0; index < fields.Length; index++)
            {
                ILevelField levelField = fields[index];
                float current = levelField.Formatter.TransformValue(levelField.AtLv(lvl));
                float against = current;
                o[index] = current;

                if (lvl > 0 && withUpgrade)
                    against = levelField.Formatter.TransformValue(levelField.AtLv(lvl - 1));

                o[index + argsLen] = current - against;
            }

            return o;
        }

        public string GetLevelDescription(int lvl, bool withUpgrade)
        {
            ILevelField[] fields;
            if (levelFields is null)
            {
                levelFields = CreateLevelFields(lvl);
                fields = levelFields;
            }
            else fields = CacheLevelFields ? levelFields : CreateLevelFields(lvl);

            int fieldsLen = fields.Length;
            object[] arguments = BuildDescriptionArguments(lvl, withUpgrade, fields);
            
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < fields.Length; i++)
            {
                var levelField = fields[i];
                sb.Append(levelField.FormatString(i, i + fieldsLen));
            }

            string text = sb.ToString();
            return Formatter.Format(text, arguments);
        }
    }
}