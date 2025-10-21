using System.Collections;
using Definitions;
using Gameplay.Genes;
using Gameplay.Interaction;
using Gameplay.Mutations;
using UI.Menus;
using UnityEngine;
using Util;

namespace Gameplay.Breeding
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EggDrop : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer minimapIcon;
        [SerializeReference] private Egg egg;
        private bool squashed;
        private bool immune = true;


        private void Start()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            
            if(squashed) return;
            egg ??= new Egg(TrioGene.Zero, new MutationData());
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
            if(immune || !col.gameObject.layer.Equals(GlobalDefinitions.EnemyPhysicsLayer)) return;
            Squash();
        }

        public void Squash()
        {
            minimapIcon.enabled = false;
            squashed = true;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = GlobalDefinitions.PuddleSprite;
            spriteRenderer.color = GlobalDefinitions.EggPuddleColor;
            spriteRenderer.sortingLayerName = "Ground";
            spriteRenderer.sortingOrder = -1;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            transform.rotation = Quaternion.identity;
            StartCoroutine(FadeRoutine(spriteRenderer));
        }

        private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer)
        {
            yield return new WaitForSeconds(5);

            float initialAlpha = spriteRenderer.color.a;
            float t = 2;
            while (t > 0)
            {
                float alpha = t / 2f;
                t -= Time.deltaTime;

                if(alpha < initialAlpha) spriteRenderer.color = spriteRenderer.color.WithAlpha(alpha);
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private IEnumerator ImmunityRoutine()
        {
            yield return new WaitForSeconds(5);
            immune = false;
        }

        
        
        // IInteractable
        public void Interact()
        {
            Player.PlayerManager.Instance.PickEgg(egg);
            Destroy(gameObject);
        }

        public bool CanInteract() => !Player.PlayerManager.Instance.IsHoldingEgg && !squashed;
        public float PopupDistance => 0.5f;
        public string ActionTitle => "Pickup egg";
        public Vector3 Position => transform.position;
    }
}