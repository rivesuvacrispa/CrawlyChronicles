using UI.Menus;

namespace Gameplay.Food.Foodbeds
{
    public class MutagenicFoodbed : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            MutationMenu.ShowForCurrentEgg();
        }
    }
}