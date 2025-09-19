using Scriptable;

namespace Gameplay.Abilities
{
    public abstract class ActiveAbility : BasicAbility
    {
        public new ActiveMutation Scriptable => (ActiveMutation) scriptable;
        public float Cooldown => Scriptable.GetCooldown(level);

        public abstract void Activate();
        public virtual bool CanActivate() => true;
        public abstract object[] GetDescriptionArguments(int lvl, bool withUpgrade);

        public override string GetLevelDescription(int lvl, bool withUpgrade) 
            => Scriptable.GetStatDescription(GetDescriptionArguments(lvl, withUpgrade));
    }
}