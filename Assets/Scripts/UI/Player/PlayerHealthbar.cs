
using UI.Elements;

namespace UI.Player
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void OnValueCatch(float health)
        {
            StartFade(1f);
        }
    }
}