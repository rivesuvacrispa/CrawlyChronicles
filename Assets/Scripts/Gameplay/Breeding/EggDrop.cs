using System.Collections;
using Definitions;
using GameCycle;
using Gameplay.Abilities;
using Genes;
using Gameplay.Interaction;
using UI;
using UnityEngine;
using Util;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EggDrop : MonoBehaviour, IInteractable
    {
        [SerializeReference] private Egg egg;
        private bool crashed;
        private bool immune = true;


        private void Start()
        {
            egg ??= new Egg(TrioGene.Zero, new MutationData());
            MainMenu.OnResetRequested += OnResetRequested;
            StartCoroutine(ImmunityRoutine());
        }

        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnResetRequested() => Destroy(gameObject);
        
        public EggDrop SetEgg(Egg eg)
        {
            egg = eg;
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
            Player.Manager.Instance.PickEgg(egg);
            Destroy(gameObject);
        }

        public bool CanInteract() => !Player.Manager.Instance.IsHoldingEgg && !crashed;
        public float PopupDistance => 0.5f;
        public string ActionTitle => "Pickup egg";
        public Vector3 Position => transform.position;
    }
}