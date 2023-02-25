using Util;

namespace UI
{
    public interface IDamageable : ITransformProvider
    {
        public float HealthbarOffsetY { get; }
        public float HealthbarWidth { get; }
    }
}