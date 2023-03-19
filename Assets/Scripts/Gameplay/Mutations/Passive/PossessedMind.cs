using System.Text;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Passive
{
    public class PossessedMind : BasicAbility
    {
        [Header("Damage taken")] 
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;

        
        
        public static float ConsumingDamage { get; private set; }
        public static bool Enabled { get; private set; }
        
        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            ConsumingDamage = LerpLevel(damageLvl1, damageLvl10, lvl);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Enabled = false;
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float dmg = LerpLevel(damageLvl1, damageLvl10, lvl);
            float prevDmg = 0;

            if (lvl > 0 && withUpgrade) 
                prevDmg = LerpLevel(damageLvl1, damageLvl10, lvl - 1);

            sb.AddAbilityLine("Damage taken", dmg, prevDmg, withUpgradePlus: false);
            
            return sb.ToString();
        }
    }
}