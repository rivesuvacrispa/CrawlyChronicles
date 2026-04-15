using Gameplay.Breeding;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class QueensFertility : ActiveAbility
    {
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat eggsAmount = new LevelFloat(new Vector2(0.5f, 3f));

        public static float CurrentEggsAmount { get; private set; }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            CurrentEggsAmount = eggsAmount.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            BreedingManager.Instance.BecomePregnant(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData());
        }

        public override bool CanActivate()
        {
            return base.CanActivate() && BreedingManager.Instance.CanBreed;
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new ILevelField[]
            {
                Scriptable.Cooldown,
                eggsAmount.UseKey(LevelFieldKeys.EGGS_AMOUNT)
            };
        }
    }
}