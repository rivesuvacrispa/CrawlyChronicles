using DG.Tweening;
using Gameplay.AI;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteSwarmer : Enemy
    {
        [SerializeField] private Locator locator;
        
        private TermiteSwarmer currentFollowTarget;
        private static readonly int ANIMATION_FLIGHT = Animator.StringToHash("TermiteSwarmerFlight");
        
        
        
        private void OnEnable()
        {
            spriteRenderer.enabled = false;
            // locator.OnTargetLocated += OnTargetLocated;
        }

        private void OnDisable() => locator.OnTargetLocated -= OnTargetLocated;

        private void OnTargetLocated(ILocatorTarget target)
        {
            if (target is not TermiteSwarmerLocatorTarget swarmerLocatorTarget) return;
            OnSwarmerLocated(swarmerLocatorTarget.TermiteSwarmer);
        }

        private void OnSwarmerLocated(TermiteSwarmer swarmer)
        {
            if (currentFollowTarget is not null) return;

            currentFollowTarget = swarmer;
            currentFollowTarget.OnProviderDestroy += OnFollowTargetDestroy;
            
            stateController.SetState(AIState.Follow, swarmer, reachDistance: 1f);
        }

        private void OnFollowTargetDestroy()
        {
            currentFollowTarget.OnProviderDestroy -= OnFollowTargetDestroy;
            currentFollowTarget = null;
        }
        
        public override void OnMapEntered()
        {
            if (!((TermiteSwarmerStateController)stateController).CanPlayFlightAnimation)
            {
                spriteRenderer.enabled = true;
                stateController.SetState(AIState.Wander);
                return;
            }

            PlayFlightAnimation();
        }

        private void PlayFlightAnimation()
        {
            stateController.SetState(AIState.None);
            stateController.SetEtherial(true);
            stateController.TakeMoveControl();
            spriteRenderer.color = spriteRenderer.color.WithAlpha(0f);
            spriteRenderer.enabled = true;

            animator.Play(ANIMATION_FLIGHT);

            int direction = Random.value < 0.5f ? -1 : 1;
            Vector3 offset = new Vector3(10, 10) * direction;
            transform.position += offset;
            if (direction < 0) spriteRenderer.flipX = true;
            spriteRenderer.DOColor(spriteRenderer.color.WithAlpha(1f), 2f);
            transform.DOMove(transform.position - offset, 2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                stateController.SetState(AIState.Wander);
                stateController.SetEtherial(false);
                stateController.ReturnMoveControl();
                PlayCrawl();
                spriteRenderer.flipX = false;
                transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            });
        }

        public override void OnPlayerLocated()
        {
            
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
            
        }

        protected override void OnDamageTaken()
        {
            AttackPlayer();
        }
    }
}