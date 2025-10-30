using Gameplay.Effects.Healthbars;
using Hitboxes;
using Util.Interfaces;

namespace UI.Player
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void OnValueCatch(float health)
        {
            StartFade(1f);
        }

        protected override void OnTargetDeath(IDamageable damageable)
        {
            
        }
    }
}