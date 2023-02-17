using System.Collections;
using Gameplay.AI;
using Gameplay.Food;
using Gameplay.Interaction;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Gameplay.Enemies
{
    public class NeutralAnt : Enemy, IContinuouslyInteractable
    {
        [SerializeField] private ParticleSystem breedingParticles;
        [SerializeField] private Animator breedAnimator;
        
        private readonly int breedAnimHash = Animator.StringToHash("NeutralAntBodyBreeding");
        private readonly int idleAnimHash = Animator.StringToHash("NeutralAntBodyIdle");

        private bool canBreed = true;
        
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
        }

        public override void OnPlayerLocated()
        {
            StopInterest();
            StartCoroutine(InterestRoutine());
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(FoodBed foodBed)
        {
        }

        protected override void OnDamageTaken()
        {
            StopInterest();
            canBreed = false;
            stateController.SetState(AIState.Flee);
        }

        public void Interact()
        {
            canBreed = false;
            BreedingManager.Instance.BecomePregnant();
        }

        public bool CanInteract() => canBreed && BreedingManager.Instance.CanBreed;

        

        // IContinuouslyInteractable
        public void OnInteractionStart()
        {
            StopInterest();
            breedAnimator.Play(breedAnimHash);
            stateController.SetState(AIState.None);
            breedingParticles.Play();
            BreedingManager.Instance.PlayBreedingAnimation();
        }

        public void OnInteractionStop()
        {
            breedAnimator.Play(idleAnimHash);
            stateController.SetState(AIState.Wander);
            breedingParticles.Stop();
            BreedingManager.Instance.PlayIdleAnimation();
            if(!canBreed) stateController.SetState(AIState.Flee);
        }

        private IEnumerator InterestRoutine()
        {
            stateController.TakeMoveControl();

            float t = 0;
            while (t < 2f)
            {
                rb.rotation = PhysicsUtility
                    .RotateTowardsPosition(rb.position, rb.rotation, Player.Movement.Position, 5);
                t += Time.deltaTime;
                yield return null;
            }
            
            stateController.ReturnMoveControl();
            stateController.SetState(AIState.Wander);
        }

        private void StopInterest()
        {
            StopCoroutine(InterestRoutine());
            stateController.ReturnMoveControl();
        }

        public float InteractionTime => 3f;
        public float PopupDistance => 0.75f;
        public string ActionTitle => "Breed";
        Vector3 IInteractable.Position => transform.position;
    }
}