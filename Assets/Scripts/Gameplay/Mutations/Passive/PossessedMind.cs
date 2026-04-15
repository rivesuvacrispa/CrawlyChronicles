using System;
using UnityEngine;
using Util.Abilities;

namespace Gameplay.Mutations.Passive
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

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return Array.Empty<ILevelField>();
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
    }
}