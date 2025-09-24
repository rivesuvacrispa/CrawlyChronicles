
namespace UI.Elements
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void OnValueCatched(float health)
        {
            currentRoutine = StartCoroutine(FadeRoutine());
        }
    }
}