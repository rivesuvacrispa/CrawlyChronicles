using Gameplay.Player;
using Scriptable;
using UI.Elements;
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


        protected static float GetPassiveProcRate(float proc) => proc * PlayerManager.PlayerStats.PassiveProcRate;
        public static float GetAbilityDamage(float damage) => damage + damage * PlayerManager.PlayerStats.AbilityDamage;

        protected virtual void Start() => SetLevel(level, true);

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

        protected virtual void OnDisable()
        {
            Button.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            if (Button is not null)
                Button.SetActive(true);
        }

        public virtual string GetLevelDescription(int lvl, bool withUpgrade) => string.Empty;
    }
}