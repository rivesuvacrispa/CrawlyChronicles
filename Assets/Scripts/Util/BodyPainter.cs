using System.Collections;
using UnityEngine;

namespace Util
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BodyPainter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private Coroutine fadeRoutine;
        private Coroutine colorRoutine;

        private Color initialColor;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialColor = spriteRenderer.color;
        }

        public Color CurrentColor => spriteRenderer.color;
        
        public void Paint(Gradient gradient, float duration)
        {
            if(colorRoutine is not null) StopCoroutine(colorRoutine);
            colorRoutine = StartCoroutine(PaintRoutine(gradient, duration));
        }

        public void FadeOut(float duration)
        {
            if(fadeRoutine is not null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeRoutine(duration, 0));
        }
        
        public void FadeIn(float duration)
        {
            if(fadeRoutine is not null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeRoutine(duration, 1));
        }
        
        private IEnumerator FadeRoutine(float duration, int direction)
        {
            Color color = spriteRenderer.color;
            float t = 0;
            while (t < duration)
            {
                SetColor(color.WithAlpha(direction == 0 ? 1 - Mathf.Clamp01(t / duration) : Mathf.Clamp01(t / duration)));
                t += Time.deltaTime;
                yield return null;
            }
            
            SetColor(color.WithAlpha(direction == 0 ? 0 : 1));
            fadeRoutine = null;
        }
        
        private IEnumerator PaintRoutine(Gradient gradient, float duration)
        {
            float t = 0;
            while (t < duration)
            {
                SetColor(gradient.Evaluate(Mathf.Clamp01(t / duration)).WithAlpha(spriteRenderer.color.a));
                t += Time.deltaTime;
                yield return null;
            }
            
            SetColor(gradient.Evaluate(1).WithAlpha(spriteRenderer.color.a));
            colorRoutine = null;
        }

        private void OnDisable() => ResetColor();

        
        
        private void SetColor(Color c) => spriteRenderer.color = c;

        public void ResetColor(Color c = default)
        {
            if (c == default) c = initialColor.WithAlpha(1);
            StopAllCoroutines();
            SetColor(c);
        }

        public void SetSortingLayer(string layer) => spriteRenderer.sortingLayerName = layer;
        public void SetSortingOrder(int order) => spriteRenderer.sortingOrder = order;
        public void SetMaterial(Material material) => spriteRenderer.material = material;
    }
}