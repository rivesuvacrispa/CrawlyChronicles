using Util.Interfaces;

namespace Gameplay.Effects.Healthbars
{
    public readonly struct HealthbarArguments
    {
        public readonly IDamageable target;

        public HealthbarArguments(IDamageable target)
        {
            this.target = target;
        }
    }
}