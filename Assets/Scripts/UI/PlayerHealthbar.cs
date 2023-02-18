namespace UI
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void Update()
        {
            base.Update();
            UpdateWidth();
        }

        protected override void OnValueCatched(float health)
        {
            if (health == 0)
            {
                //TODO: IDK BRO
            } else if (health >= 1 - float.Epsilon)
            {
                currentRoutine = StartCoroutine(FadeRoutine());
            }
        }
    }
}