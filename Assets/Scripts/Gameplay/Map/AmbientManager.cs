using System;
using Timeline;
using UnityEngine;

namespace Gameplay.Map
{
    public class AmbientManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fireflyParticles;

        
        
        private void OnEnable()
        {
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
        }

        private void OnDisable()
        {
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void OnDayStart(int daycounter)
        {
            fireflyParticles.Stop();
        }

        private void OnNightStart(int daycounter)
        {
            fireflyParticles.Play();
        }
    }
}