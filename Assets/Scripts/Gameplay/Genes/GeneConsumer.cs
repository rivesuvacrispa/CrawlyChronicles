using System.Collections;
using Definitions;
using Player;
using SoundEffects;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Genes
{
    [RequireComponent(typeof(Light2D))]
    public class GeneConsumer : MonoBehaviour
    {
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalSpriteColor;
        
        private new Light2D light;
        private float initialIntensity;
        private Coroutine routine;

        private void Start()
        {
            light = GetComponent<Light2D>();
            light.enabled = false;
            initialIntensity = light.intensity;
        }
        
        public void ConsumeGene(GeneType geneType)
        {
            Color color = GlobalDefinitions.GetGeneColor(geneType);
            if(routine is not null) StopCoroutine(routine);
            routine = StartCoroutine(ConsumingRoutine(color, 0.5f));
            PlayerAudioController.Instance.PlayGenePickup();
        }

        private IEnumerator ConsumingRoutine(Color color, float duration)
        {
            hitbox.BlockColorChange = true;
            light.intensity = initialIntensity;
            light.color = color;
            light.enabled = true;
            spriteRenderer.color = color;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new[]
                {
                    new GradientColorKey(color, 0),
                    new GradientColorKey(normalSpriteColor, 1)
                }, new[]
                {
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1),
                });
            
            float t = 0;
            while (t < duration)
            {
                float value = t / duration;
                Color currentColor = grad.Evaluate(value);
                light.color = currentColor;
                light.intensity = initialIntensity * (1 - value);
                spriteRenderer.color = currentColor;
                t += Time.deltaTime;
                yield return null;
            }

            hitbox.BlockColorChange = false;
            light.enabled = false;
            spriteRenderer.color = normalSpriteColor;
            routine = null;
        }
    }
}