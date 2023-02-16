namespace UI
{
    public class PlayerHealthbar : Healthbar
    {
        protected override void Update()
        {
            base.Update();
            UpdateWidth();
        }

        protected override void OnValueCatched(int health)
        {
            if (health == 0)
            {
                //TODO: IDK BRO
            } else if (health == Player.Manager.PlayerStats.MaxHealth)
            {
                currentRoutine = StartCoroutine(FadeRoutine());
            }
        }
    }
}