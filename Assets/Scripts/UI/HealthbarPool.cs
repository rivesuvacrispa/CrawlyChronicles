using Definitions;
using UnityEngine;
using Util.Interfaces;

namespace UI
{
    public class HealthbarPool : MonoBehaviour
    {
        public static HealthbarPool Instance { get; private set; }

        [SerializeField] private Healthbar healthbarPrefab;

        private HealthbarPool() => Instance = this;

        public Healthbar Create(IDamageable damageable)
        {
            Healthbar hb = Instantiate(healthbarPrefab, GlobalDefinitions.WorldCanvasTransform);
            hb.SetTarget(damageable);
            return hb;
        }
    }
}