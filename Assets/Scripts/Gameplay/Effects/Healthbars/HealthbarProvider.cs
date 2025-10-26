using Pooling;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Effects.Healthbars
{
    [RequireComponent(typeof(IDamageable))]
    public class HealthbarProvider : MonoBehaviour
    {
        private void Start()
        {
            IDamageable damageable = gameObject.GetComponent<IDamageable>();
            PoolManager.GetEffect<Healthbar>(new HealthbarArguments(damageable));
            Debug.Log($"Provided healthbar for: {gameObject.name}");
        }
    }
}