namespace Gameplay.Effects.Fly
{
    public readonly struct FlyArguments
    {
        public readonly float damage;
        public readonly float attackCooldown;
        public readonly float flySpeed;
        public readonly float rotationSpeed;

        public FlyArguments(float damage, float attackCooldown, float flySpeed, float rotationSpeed)
        {
            this.damage = damage;
            this.attackCooldown = attackCooldown;
            this.flySpeed = flySpeed;
            this.rotationSpeed = rotationSpeed;
        }
    }
}