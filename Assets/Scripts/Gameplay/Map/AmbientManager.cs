using System;
using Timeline;
using UI.Elements;
using UnityEngine;

namespace Gameplay.Map
{
    public class AmbientManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fireflyParticles;

        
        
        private void OnEnable()
        {
            AmbientToggleButton.OnToggled += OnAmbientToggled;
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
            if (gameObject.activeInHierarchy) fireflyParticles.Play();
        }

        private void OnAmbientToggled(bool state) => gameObject.SetActive(state);
    }
}