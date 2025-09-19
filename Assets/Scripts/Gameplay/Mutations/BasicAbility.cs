using Player;
using Scriptable;
using UI;
using UnityEngine;

namespace Gameplay.Mutations
{
    public class BasicAbility : MonoBehaviour
    {
        [SerializeField] private AbilityController abilityController;
        [SerializeField] protected BasicMutation scriptable;
        [SerializeField, Range(0, 9)] protected int level;

        public BasicMutation Scriptable => scriptable;
        public BasicAbilityButton Button { get; set; }
        public int Level => level;
        public bool Learned { get; private set; }


        protected static float GetPassiveProcRate(float proc) => proc * PlayerManager.PlayerStats.PassiveProcRate;
        protected static float GetAbilityDamage(float damage) => damage + damage * PlayerManager.PlayerStats.AbilityDamage;

        private void Awake() => abilityController.CreateUIElement(this);

        protected virtual void Start() => SetLevel(level, true);

        public void SetLevel(int newLevel, bool forceUpdate = false)
        {
            if(newLevel == level && !forceUpdate) return;
            level = Mathf.Clamp(newLevel, 0, 9);
            Button.UpdateLevelText(level);
            OnLevelChanged(level);
        }

        protected static float LerpLevel(float from, float to, int lvl) => Mathf.Lerp(from, to, lvl / 9f);
        
        public virtual void OnLevelChanged(int lvl)
        {
            if(Button is not null) Button.UpdateLevelText(lvl);
        }

        protected virtual void OnDisable()
        {
            Learned = false;
            Button.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            Learned = true;
            Button.SetActive(true);
        }

        public virtual string GetLevelDescription(int lvl, bool withUpgrade) => string.Empty;
    }
}