using UnityEngine;

namespace Scriptable.Enemies
{
    [CreateAssetMenu(menuName = "Scriptable/Enemy/DiggingWasp")]
    public class DiggingWasp : Enemy
    {
        [SerializeField] private GameObject eggBedPrefab;
        [SerializeField] private float diggingTime;
        [SerializeField] private float diggingCooldown;

        public GameObject EggBedPrefab => eggBedPrefab;
        public float DiggingTime => diggingTime;
        public float DiggingCooldown => diggingCooldown;
    }
}