using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.AI;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Food;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies.Enemies
{
    public class TermiteSwarmer : Enemy
    {
        [SerializeField] private Locator locator;
        [SerializeField] private ParticleSystem breedingParticles;
        
        public TermiteSwarmer BreedTarget { get; private set; }
        public bool Breeding { get; private set; }
        public bool BreedOnCooldown { get; private set; }
        public bool CanBreed { get; private set; } = true;
        public bool HasBreedTarget => BreedTarget is not null;
        private static readonly int ANIMATION_FLIGHT = Animator.StringToHash("TermiteSwarmerFlight");
        public delegate void BreedingEvent(TermiteSwarmer actor);
        public event BreedingEvent OnBreedingInterrupt;
        private Tween currentTween;


        
        
        private void OnEnable()
        {
            spriteRenderer.enabled = false;
            locator.OnTargetLocated += OnTargetLocated;
        }

        private void OnDisable() => locator.OnTargetLocated -= OnTargetLocated;

        public override void Die()
        {
            base.Die();
            if (Breeding) OnBreedingInterrupt?.Invoke(this);
        }

        private void OnTargetLocated(ILocatorTarget target)
        {
            if (target is not TermiteSwarmerLocatorTarget swarmerLocatorTarget) return;
            OnSwarmerLocated(swarmerLocatorTarget.TermiteSwarmer);
        }

        private void OnFollowTargetDestroy(IDestructionEventProvider target)
        {
            target.OnProviderDestroy -= OnFollowTargetDestroy;
            BreedTarget = null;
        }

        public override void OnMapEntered()
        {
            if (!((TermiteSwarmerStateController)stateController).CanPlayFlightAnimation)
            {
                spriteRenderer.enabled = true;
                stateController.SetState(AIState.Wander);
                stateController.UndismissLocator();
                return;
            }

            PlayFlightAnimation();
        }

        private void PlayFlightAnimation()
        {
            stateController.DismissLocator();
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
                stateController.UndismissLocator();
                PlayCrawl();
                spriteRenderer.flipX = false;
                transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            });
        }

        private void OnSwarmerLocated(TermiteSwarmer swarmer)
        {
            if (BreedOnCooldown ||
                !CanBreed || 
                Breeding || 
                HasBreedTarget) return;
            
            if ( (swarmer.HasBreedTarget && !swarmer.BreedTarget.Equals(this)) ||
                swarmer.Breeding || 
                !swarmer.CanBreed ||
                swarmer.BreedOnCooldown) return;

            BreedTarget = swarmer;
            BreedTarget.OnProviderDestroy += OnFollowTargetDestroy;
            
            stateController.SetState(AIState.Follow, swarmer, () =>
            {
                StartBreeding(swarmer).Forget();
            }, 1f);
        }

        public async UniTask StartBreeding(TermiteSwarmer swarmer)
        {
            if (Breeding) return;

            Breeding = true;
            swarmer.StartBreeding(this).Forget();
            stateController.SetState(AIState.None);
            swarmer.OnBreedingInterrupt += OnPartnerInterrupt;
            breedingParticles.Play();
            
            stateController.TakeMoveControl();
            currentTween = transform.DORotate(Vector3.forward * 360, 1f, RotateMode.LocalAxisAdd).SetLoops(3);
            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            StopBreeding(true);
        }

        private void StopBreeding(bool success)
        {
            if (!Breeding) return;

            currentTween?.Kill();
            stateController.ReturnMoveControl();
            BreedTarget = null;
            breedingParticles.Stop();
            Breeding = false;
            stateController.SetState(AIState.Wander);
            BreedOnCooldown = true;
            CanBreedTask().Forget();
        }

        private async UniTask CanBreedTask()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(15f),
                cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            
            BreedOnCooldown = false;
            stateController.SetState(AIState.Wander);
        }

        private void OnPartnerInterrupt(TermiteSwarmer actor)
        {
            actor.OnBreedingInterrupt += OnPartnerInterrupt;
            StopBreeding(false);
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            if (BreedOnCooldown) AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
            
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
            
        }

        protected override void OnDamageTaken()
        {
            StopBreeding(false);
            AttackPlayer();
            OnBreedingInterrupt?.Invoke(this);
            CanBreed = false;
        }
    }
}