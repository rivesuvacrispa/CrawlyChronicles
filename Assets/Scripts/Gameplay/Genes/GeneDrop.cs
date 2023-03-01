using System.Collections;
using Definitions;
using GameCycle;
using Gameplay;
using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Genes
{
    public class GeneDrop : MonoBehaviour
    {
        [SerializeField] private new Light2D light;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GeneType geneType = GeneType.Adaptive;
        [SerializeField] private int amount = -1;

        private void Start()
        {
            if(amount == -1) SetData(geneType, 1);
            StartCoroutine(SpawnRoutine());
            MainMenu.OnResetRequested += OnResetRequested;
        }
        
        public GeneDrop SetData(GeneType newType, int count)
        {
            amount = count;
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
                Vector2 pos = transform.position;
                Vector2 playerpos = Player.Movement.Position;
                Vector2 direction = playerpos - pos;
                distanceFromPlayer = direction.sqrMagnitude;
                // transform.Translate(direction.normalized * Time.deltaTime * (pickUpDistance / distanceFromPlayer));
                transform.position = Vector2.MoveTowards(pos, playerpos,
                    Time.deltaTime * (pickUpDistance / distanceFromPlayer));
                yield return null;
            }

            StatRecorder.genesCollected++;
            BreedingManager.Instance.AddGene(geneType, amount);
            Destroy(gameObject);
        }

        private IEnumerator SpawnRoutine()
        {
            enabled = false;
            yield return new WaitForSeconds(0.75f);
            enabled = true;
        }
        
        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnResetRequested() => Destroy(gameObject);
    }
}