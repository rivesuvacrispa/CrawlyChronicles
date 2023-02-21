namespace UI
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void OnValueCatched(float health)
        {
            currentRoutine = StartCoroutine(FadeRoutine());
        }
    }
}