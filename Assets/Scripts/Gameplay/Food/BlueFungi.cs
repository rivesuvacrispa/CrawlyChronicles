using Definitions;

namespace Gameplay.Food
{
    public class BlueFungi : FoodBed
    {
        protected override void OnEatenByPlayer()
        {
            Player.Manager.Instance.AddHealthPercent(0.1f);
            GlobalDefinitions.CreateRandomGeneDrop(Position);
        }
    }
}