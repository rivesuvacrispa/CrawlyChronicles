using UnityEngine;

namespace Scriptable.Enemies
{
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