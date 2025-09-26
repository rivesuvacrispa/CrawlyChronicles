namespace Gameplay.Food.Foodbeds
{
    public class SmallCacti : Foodbed
    {
        protected override void OnEatenByPlayer()
        {
            Player.PlayerManager.Instance.AddHealthPercent(0.1f);
        }

        public override bool CanSpawn(float random) => true;
    }
}