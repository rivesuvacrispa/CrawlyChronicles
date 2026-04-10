using Gameplay.Player;

namespace Gameplay.Food.Foodbeds
{
    public class MutagenicFoodbed : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            MutationManager.Instance.MutateFromPlayer();
        }
    }
}