using Player;
using Scriptable;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] protected ActiveMutation scriptable;
        [SerializeField, Range(0, 9)] private int level;
        
        public ActiveMutation Scriptable => scriptable;
        public float Cooldown => scriptable.GetCooldown(level);
        public int Level => level;
        
        
        private void Start()
        {
             GetComponentInParent<AbilityController>().CreateButton(this);
             SetLevel(level);
        }

        private void SetLevel(int newLevel)
        {
            if(newLevel == level) return;
            int previous = level;
            level = Mathf.Clamp(newLevel, 0, 9);
            if(previous != level) OnLevelChanged(level);
        }

        protected float LerpLevel(float from, float to, int lvl) => Mathf.Lerp(from, to, lvl / 9f);

        public void LevelUp() => SetLevel(level + 1);

        public abstract void OnLevelChanged(int lvl);
        public abstract void Activate();
    }
}