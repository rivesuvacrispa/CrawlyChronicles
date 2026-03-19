using Hitboxes;

namespace Gameplay.Effects.Healthbars
{
    public class TargetDummyHealthbarProvider : HealthbarProvider
    {
        private void OnEnable()
        {
            if (gameObject.TryGetComponent(out IDamageable damageable))
                damageable.OnDeath += OnDeath;
        }

        private void OnDeath(IDamageable damageable)
        {
            ProvideHealthbar();
        }

        private void OnDisable()
        {
            if (gameObject.TryGetComponent(out IDamageable damageable))
                damageable.OnDeath -= OnDeath;
        }
    }
}