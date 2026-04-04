namespace Gameplay.Food.Foodbeds
{
    public class RestoringFoodbed : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            Player.PlayerManager.Instance.AddHealthPercent(0.1f);
        }
    }
}