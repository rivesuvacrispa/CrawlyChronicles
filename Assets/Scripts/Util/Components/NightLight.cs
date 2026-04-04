using Timeline;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Util.Components
{
    public class NightLight : MonoBehaviour
    {
        private new Light2D light;
        
        private void Start()
        {
            light = GetComponent<Light2D>();
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
            if(TimeManager.IsDay) OnDayStart(0);
            else OnNightStart(0);
        }

        private void OnDestroy()
        {
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void OnDayStart(int _) => light.enabled = false;
        private void OnNightStart(int _) => light.enabled = true;
    }
}