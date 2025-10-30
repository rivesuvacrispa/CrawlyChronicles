using System;

namespace Util.Interfaces
{
    public readonly struct DamageSource
    {
        public readonly IDamageSource owner;
        private readonly int projectileID;

        public DamageSource(IDamageSource owner, int projectileID = 0)
        {
            this.owner = owner;
            this.projectileID = projectileID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(owner.GetHashCode(), projectileID);
        }
    }
}