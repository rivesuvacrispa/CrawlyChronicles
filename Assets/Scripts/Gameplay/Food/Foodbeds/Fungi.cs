using Timeline;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Food
{
    public abstract class Fungi : Foodbed
    {
        private new Light2D light;
        
        protected override void Start()
        {
            light = GetComponent<Light2D>();
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
            if(TimeManager.IsDay) OnDayStart(0);
            else OnNightStart(0);
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void OnDayStart(int _) => light.enabled = false;
        private void OnNightStart(int _) => light.enabled = true;
    }
}