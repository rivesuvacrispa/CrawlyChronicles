using Definitions;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class HealthbarPool : MonoBehaviour
    {
        public static HealthbarPool Instance { get; private set; }

        [FormerlySerializedAs("healthBarPrefab")] 
        [SerializeField] private Healthbar healthbarPrefab;

        private void Awake() => Instance = this;

        public Healthbar Create(IDamageable damageable)
        {
            Healthbar hb = Instantiate(healthbarPrefab, GlobalDefinitions.WorldCanvasTransform);
            hb.SetTarget(damageable);
            return hb;
        }
    }
}