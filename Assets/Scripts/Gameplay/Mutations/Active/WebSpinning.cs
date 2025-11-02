namespace Gameplay.Mutations.Active
{
    public class WebSpinning : ActiveAbility
    {
        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}