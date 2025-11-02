using Gameplay.Player;
using Scriptable;
using Timeline;
using UnityEngine;

namespace Gameplay.Mutations
{
    public abstract class ActiveAbility : BasicAbility
    {
        public delegate void CooldownEvent(float currentCooldown, float baseCooldown);
        public event CooldownEvent OnCooldownChanged;
        public new ActiveMutation Scriptable => (ActiveMutation) scriptable;
        public float BaseCooldown => Scriptable.GetCooldown(level);
        public bool Autocast { get; set; }

        protected float CurrentCooldown
        {
            get => currentCooldown;
            set
            {
                currentCooldown = value;
                OnCooldownChanged?.Invoke(currentCooldown, BaseCooldown);
            }
        }

        private float currentCooldown;

        

        protected abstract object[] GetDescriptionArguments(int lvl, bool withUpgrade);

        private void Update()
        {
            if (CurrentCooldown <= 0)
            {
#if UNITY_EDITOR
                // In god mode, autocast cannot be enabled due to zero abilities cooldown
                if (PlayerManager.Instance.GodMode) return;
#endif
                
                if (Autocast && !TimeManager.IsDay && CanActivate()) Activate(true);
                return;
            }

            CurrentCooldown -= Time.deltaTime;
        }
        
        public virtual void Activate(bool auto = false)
        {
            SetOnCooldown();
        }

        protected void SetOnCooldown()
        {
#if UNITY_EDITOR
            // Do not set on cooldown in god mode
            if (PlayerManager.Instance.GodMode)
            {
                CurrentCooldown = 0;
                return;
            };
#endif
            
            CurrentCooldown = BaseCooldown;
        }

        public virtual bool CanActivate() => CurrentCooldown <= 0;

        public override string GetLevelDescription(int lvl, bool withUpgrade) 
            => Scriptable.GetStatDescription(GetDescriptionArguments(lvl, withUpgrade));

        protected override void OnDisable()
        {
            base.OnDisable();
            CurrentCooldown = 0;
        }
    }
}