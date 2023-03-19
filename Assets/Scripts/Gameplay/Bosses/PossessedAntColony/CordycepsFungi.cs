using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay.Bosses.PossessedAntColony
{
    public class CordycepsFungi : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer fungiSpriteRenderer;
        [SerializeField] private SpriteRenderer corpseSpriteRenderer;
        [SerializeField] private new Light2D light;
        [SerializeField] private ParticleSystem sporesParticleSystem;

        private int growthStage;
        private float stageGrowthTime;
        private PossessedAntColonyDefinitions definitions;
        
        private void Start()
        {
            stageGrowthTime = CordycepsExpansion.Instance.CordycepsGrowthTime / 3f;
            SetGrowthStage(0);
            StartCoroutine(GrowthRoutine());
        }

        private IEnumerator GrowthRoutine()
        {
            while (growthStage < 3)
            {
                yield return new WaitForSeconds(stageGrowthTime * Random.Range(0.9f, 1.1f));
                SetGrowthStage(growthStage + 1);
            }
        }

        private IEnumerator SporesRoutine()
        {
            sporesParticleSystem.Play();
            light.enabled = true;
            while (enabled)
            {
                CordycepsExpansion.Instance.AddExpansion(1);
                yield return new WaitForSeconds(1f);
            }
        }

        private void SetGrowthStage(int stage)
        {
            growthStage = stage;
            fungiSpriteRenderer.sprite = CordycepsExpansion.Instance.GetCordycepsSprite(growthStage);
            if (growthStage == 3) StartCoroutine(SporesRoutine());
        }

        private void Die() => Destroy(gameObject);
    }
}