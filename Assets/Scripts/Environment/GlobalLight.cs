using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Environment
{
    [RequireComponent(typeof(Light2D))]
    public class GlobalLight : MonoBehaviour
    {
        [SerializeField] private Vector2 intensity;
        [SerializeField] private Gradient color;
        [SerializeField] private int transitionTimeSeconds;
        
        private new Light2D light;


        private void Awake() => light = GetComponent<Light2D>();
        
        public void SetDay()
        {
            StopAllCoroutines();
            StartCoroutine(TransitionRoutine(false));
        }
        
        public void SetNight()
        {
            StopAllCoroutines();
            StartCoroutine(TransitionRoutine(true));
        }

        
        public void SetInstantly(bool intoNight)
        {
            light ??= GetComponent<Light2D>();
            int isNight = intoNight ? 0 : 1;
            light.color = color.Evaluate(isNight);
            light.intensity = Mathf.Lerp(intensity.x, intensity.y, isNight);
        }

        private IEnumerator TransitionRoutine(bool intoNight)
        {
            float t = 0f;
            
            while (t < transitionTimeSeconds)
            {
                float value = t / transitionTimeSeconds;
                if (intoNight) value = 1 - value;
                light.color = color.Evaluate(value);
                light.intensity = Mathf.Lerp(intensity.x, intensity.y, value);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}