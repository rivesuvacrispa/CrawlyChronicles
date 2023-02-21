using Player;
using UI;

namespace Gameplay.Food
{
    public class RadioactiveFungi : FoodBed
    {
        protected override void OnEatenByPlayer()
        {
            MutationMenu.Show(MutationTarget.Player, 
                new Egg(BreedingManager.Instance.TrioGene, AbilityController.GetMutationData()));
        }
    }
}