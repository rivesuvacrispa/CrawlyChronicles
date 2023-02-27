using UnityEngine;

namespace Util
{
    public interface IDamageGradientSupplier
    {
        public Gradient DamageGradient { get; }
    }
}