using System.Text;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Active
{
    public class QueensFertility : ActiveAbility
    {
        [Header("Eggs amount")]
        [SerializeField] private float eggsAmountLvl1;
        [SerializeField] private float eggsAmountLvl10;

        public static float EggsAmount { get; private set; }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            EggsAmount = LerpLevel(eggsAmountLvl1, eggsAmountLvl10, lvl);
        }

        public override void Activate()
        {
            if(BreedingManager.Instance.CanBreed)
                BreedingManager.Instance.BecomePregnant(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData());
        }
        
        public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            StringBuilder sb = new StringBuilder();

            float prevCd = 0;
            float eggs = LerpLevel(eggsAmountLvl1, eggsAmountLvl10, lvl) * 0.25f;
            float prevEggs = 0;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevEggs = LerpLevel(eggsAmountLvl1, eggsAmountLvl10, prevLvl) * 0.25f;
            }
            
            sb.AddAbilityLine("Cooldown", Scriptable.GetCooldown(lvl), prevCd, false, suffix: "s");
            sb.AddAbilityLine("Chance to lay more eggs", eggs, prevEggs, percent: true);
            
            return sb.ToString();
        }
    }
}