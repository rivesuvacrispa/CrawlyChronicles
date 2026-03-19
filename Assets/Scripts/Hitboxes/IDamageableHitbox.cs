using Util.Interfaces;

namespace Hitboxes
{
    public interface IDamageableHitbox
    {
        public bool Dead { get; }
        public bool ImmuneToSource(DamageSource source);
        public void Hit(DamageInstance instance);
        public void Die();
    }
}