using Gameplay.Breeding;
using Gameplay.Player;
using UI.Menus;

namespace Gameplay.Food.Foodbeds
{
    public class MutagenicFoodbed : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            MutationMenu.Show(MutationTarget.Player, 
                new Egg(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData()));
        }
    }
}