using System.Collections;
using Definitions;
using Genes;
using Gameplay.Interaction;
using UnityEngine;
using Util;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Egg : MonoBehaviour, IInteractable
    {
        private TrioGene genes;
        private bool crashed;
        private bool immune = true;


        private void Start() => StartCoroutine(ImmunityRoutine());

        public Egg SetGenes(TrioGene gene)
        {
            genes = gene;
            return this;
        }


        private void OnCollisionEnter2D(Collision2D col)
        {
            if(immune) return;
            if (col.gameObject.layer.Equals(GlobalDefinitions.EnemyPhysicsLayerMask))
            {
                var spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = GlobalDefinitions.PuddleSprite;
                spriteRenderer.color = GlobalDefinitions.EggPuddleColor;
                spriteRenderer.sortingOrder = -1;
                GetComponent<Collider2D>().enabled = false;
                GetComponent<Rigidbody2D>().simulated = false;
                crashed = true;
                StartCoroutine(FadeRoutine(spriteRenderer));
            }
        }

        private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer)
        {
            yield return new WaitForSeconds(15);

            float initialAlpha = spriteRenderer.color.a;
            float t = 0;
            while (t < 2)
            {
                float alpha = 1 - t;
                t += Time.deltaTime;

                if(alpha > initialAlpha) continue;
                spriteRenderer.color = spriteRenderer.color.WithAlpha(alpha);
            }
            
            Destroy(gameObject);
        }

        private IEnumerator ImmunityRoutine()
        {
            yield return new WaitForSeconds(3);
            immune = false;
        }

        // IInteractable
        public void Interact()
        {
            Player.Manager.Instance.PickEgg(genes);
            Destroy(gameObject);
        }

        public bool CanInteract() => !Player.Manager.Instance.IsHoldingEgg && !crashed;
        public float PopupDistance => 0.5f;
        public string ActionTitle => "Pickup egg";
        public Vector3 Position => transform.position;
    }
}