using System.Collections;
using UnityEngine;

namespace Util
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BodyPainter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        
        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        public void Paint(Gradient gradient, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(PaintRoutine(gradient, duration));
        }

        public void Fade(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(duration));
        }

        private IEnumerator FadeRoutine(float duration)
        {
            Color color = spriteRenderer.color;
            SetColor(color.WithAlpha(1));
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                SetColor(color.WithAlpha(1 - Mathf.Clamp01(t / duration)));                
                yield return null;
            }
            
            SetColor(color.WithAlpha(0));
        }
        
        private IEnumerator PaintRoutine(Gradient gradient, float duration)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                SetColor(gradient.Evaluate(Mathf.Clamp01(t / duration)));                
                yield return null;
            }
            
            SetColor(gradient.Evaluate(1));
        }

        private void SetColor(Color c) => spriteRenderer.color = c;
    }
}