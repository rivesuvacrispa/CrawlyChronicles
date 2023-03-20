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

        public override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            float cd = Scriptable.GetCooldown(lvl);
            float prevCd = cd;
            int eggs = (int) (LerpLevel(eggsAmountLvl1, eggsAmountLvl10, lvl) * 0.25f * 100);
            int prevEggs = eggs;

            if (lvl > 0 && withUpgrade)
            {
                var prevLvl = lvl - 1;
                prevCd = Scriptable.GetCooldown(prevLvl);
                prevEggs = (int) (LerpLevel(eggsAmountLvl1, eggsAmountLvl10, prevLvl) * 0.25f * 100);
            }
            
            return new object[]
            {
                cd,          eggs,
                cd - prevCd, eggs - prevEggs
            };
        }
    }
}