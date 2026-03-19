using Hitboxes;
using Pooling;
using UnityEngine;

namespace Gameplay.Effects.Healthbars
{
    [RequireComponent(typeof(IDamageable))]
    public class HealthbarProvider : MonoBehaviour
    {
        private void Start()
        {
            ProvideHealthbar();
        }

        protected void ProvideHealthbar()
        {
            IDamageable damageable = gameObject.GetComponent<IDamageable>();
            PoolManager.GetEffect<Healthbar>(new HealthbarArguments(damageable));
        }
    }
}