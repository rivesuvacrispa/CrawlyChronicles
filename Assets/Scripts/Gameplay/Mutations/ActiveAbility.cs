using Scriptable;

namespace Gameplay.Abilities
{
    public abstract class ActiveAbility : BasicAbility
    {
        public new ActiveMutation Scriptable => (ActiveMutation) scriptable;
        public float Cooldown => Scriptable.GetCooldown(level);

        public abstract void Activate();
    }
}