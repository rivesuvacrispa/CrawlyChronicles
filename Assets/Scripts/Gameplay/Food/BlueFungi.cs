using Definitions;

namespace Gameplay.Food
{
    public class BlueFungi : FoodBed
    {
        protected override void OnEatenByPlayer()
        {
            Player.Manager.Instance.AddHealth(1.5f);
            GlobalDefinitions.CreateRandomGeneDrop(Position);
        }
    }
}