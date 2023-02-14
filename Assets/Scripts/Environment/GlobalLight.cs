using System.Collections;
using Timeline;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Environment
{
    [RequireComponent(typeof(Light2D))]
    public class GlobalLight : MonoBehaviour
    {
        [SerializeField] private Gradient color;
        [SerializeField] private int transitionTimeSeconds;
        
        private new Light2D light;


        private void Awake()
        {
            light = GetComponent<Light2D>();
            TimeManager.OnDayStart += SetDay;
            TimeManager.OnNightStart += SetNight;
        }

        private void OnDestroy()
        {
            TimeManager.OnDayStart -= SetDay;
            TimeManager.OnNightStart -= SetNight;
        }

        private void SetDay(int dayCounter)
        {
            if (dayCounter == 1)
                SetInstantly(false);
            else
            {
                StopAllCoroutines();
                StartCoroutine(TransitionRoutine(false));
            }
        }
        
        private void SetNight(int dayCounter)
        {
            StopAllCoroutines();
            StartCoroutine(TransitionRoutine(true));
        }

        
        public void SetInstantly(bool intoNight)
        {
            light ??= GetComponent<Light2D>();
            int isNight = intoNight ? 0 : 1;
            light.color = color.Evaluate(isNight);
        }

        private IEnumerator TransitionRoutine(bool intoNight)
        {
            float t = 0f;
            
            while (t < transitionTimeSeconds)
            {
                float value = t / transitionTimeSeconds;
                if (intoNight) value = 1 - value;
                light.color = color.Evaluate(value);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}