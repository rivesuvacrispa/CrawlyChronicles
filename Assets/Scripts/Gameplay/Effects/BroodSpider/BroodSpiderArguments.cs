namespace Gameplay.Effects.BroodSpider
{
    public readonly struct BroodSpiderArguments
    {
        public readonly float lifetime;
        public readonly float speed;
        public readonly float damage;

        public BroodSpiderArguments(float lifetime, float speed, float damage)
        {
            this.lifetime = lifetime;
            this.speed = speed;
            this.damage = damage;
        }
    }
}