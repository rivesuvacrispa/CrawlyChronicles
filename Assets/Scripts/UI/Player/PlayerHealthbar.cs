
using UI.Elements;

namespace UI.Player
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void OnValueCatched(float health)
        {
            currentRoutine = StartCoroutine(FadeRoutine());
        }
    }
}