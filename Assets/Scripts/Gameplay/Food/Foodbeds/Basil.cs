using Timeline;

namespace Gameplay.Food
{
    public class Basil : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            Player.PlayerManager.Instance.AddHealthPercent(0.1f);
        }

        public override bool CanSpawn(float rnd) => TimeManager.IsDay;
    }
}