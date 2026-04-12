using System.Threading;
using Gameplay.Player;
using Hitboxes;
using Scriptable;
using UI.Elements;
using UnityEngine;
using Util;
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
        public int Level => scriptable.NotUpgradeable ? 9 : level;
        private ParticleCollisionProvider[] collisionProviders;
        private ParticleTriggerProvider[] triggerProviders;


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
            if(newLevel == Level && !forceUpdate) return;
            level = Mathf.Clamp(newLevel, 0, 9);
            Button.UpdateLevelText(Level);
            OnLevelChanged(Level);
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
        
        public static float CalculateAbilityDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.AbilityDamage);

        public static float CalculateSummonDamage(float baseDamage) =>
            baseDamage * (1 + PlayerManager.PlayerStats.SummonDamage);
        
        public static int CalculateSummonsAmount(int baseAmount) =>
            baseAmount + PlayerManager.PlayerStats.BonusSummonAmount;
    }
}