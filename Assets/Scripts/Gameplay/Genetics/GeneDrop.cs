using Definitions;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Genetics
{
    public class GeneDrop : MonoBehaviour
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GeneType geneType = GeneType.Neutral;

        private void Start()
        {
            SetGeneType(geneType);
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
                transform.Translate(direction.normalized * Time.deltaTime * (pickUpDistance / distanceFromPlayer));
                if (distanceFromPlayer <= 0.15f)
                {
                    BreedingManager.Instance.AddGene(geneType);
                    Destroy(gameObject);
                }
            }
        }
    }
}