using Util.Interfaces;

namespace Pooling
{
    public readonly struct ChainLightningArguments
    {
        public readonly float damage;
        public readonly float chainRange;
        public readonly float maxNumberOfJumps;
        public readonly IDamageable currentTarget;
        public readonly int currentJump;

        public ChainLightningArguments(float damage, float chainRange, float maxNumberOfJumps, IDamageable currentTarget, int currentJump)
        {
            this.damage = damage;
            this.chainRange = chainRange;
            this.maxNumberOfJumps = maxNumberOfJumps;
            this.currentTarget = currentTarget;
            this.currentJump = currentJump;
        }
    }
}