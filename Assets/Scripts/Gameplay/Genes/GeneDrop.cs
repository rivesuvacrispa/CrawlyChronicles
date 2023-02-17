using System.Collections;
using Definitions;
using Gameplay;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Genes
{
    public class GeneDrop : MonoBehaviour
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GeneType geneType = GeneType.Neutral;

        private void Start()
        {
            SetGeneType(geneType);
            enabled = false;
            StartCoroutine(SpawnRoutine());
        }
        
        public GeneDrop SetGeneType(GeneType newType)
        {
            geneType = newType;
            Color geneColor = GlobalDefinitions.GetGeneColor(newType);
            light.color = geneColor;
            spriteRenderer.color = geneColor;
            return this;
        }
        
        private void Update()
        {
            Vector2 direction = Player.Movement.Position - (Vector2) transform.position;
            float distanceFromPlayer = direction.sqrMagnitude;
            float pickUpDistance = GlobalDefinitions.GenePickupDistance;
            if (distanceFromPlayer <= pickUpDistance)
            {
                enabled = false;
                StartCoroutine(ConsumingRoutine(pickUpDistance, distanceFromPlayer));
            }
        }

        private IEnumerator ConsumingRoutine(float pickUpDistance, float distanceFromPlayer)
        {
            while (distanceFromPlayer >= 0.15f)
            {
                Vector2 direction = Player.Movement.Position - (Vector2) transform.position;
                distanceFromPlayer = direction.sqrMagnitude;
                transform.Translate(direction.normalized * Time.deltaTime * (pickUpDistance / distanceFromPlayer));
                yield return null;
            }
            
            BreedingManager.Instance.AddGene(geneType);
            Destroy(gameObject);
        }

        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(0.75f);
            enabled = true;
        }
    }
}