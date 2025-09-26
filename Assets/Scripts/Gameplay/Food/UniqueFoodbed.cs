namespace Gameplay.Food
{
    public abstract class UniqueFoodbed : Foodbed
    {
        private static bool existing;

        protected override void Start()
        {
            base.Start();
            existing = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            existing = false;
        }
        
        public override bool CanSpawn(float random) => existing == false;
        
        protected override bool CreateNotification => false;
    }
}