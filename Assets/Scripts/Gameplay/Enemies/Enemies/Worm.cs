using System.Collections;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Timeline;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies.Enemies
{
    public class Worm : Enemy
    {
        [SerializeField] private ParticleSystem dirtParticles;

        private static readonly int DiggingInAnimHash = Animator.StringToHash("WormDiggingIn");
        private static readonly int DiggingOutAnimHash = Animator.StringToHash("WormDiggingOut");

        private bool digged;
        private bool hungry = true;

        private Coroutine diggingRoutine;
        private Coroutine digDelayRoutine;
        
        
        public override void OnMapEntered() => DigIn(0f);

        public override void OnPlayerLocated()
        {
        }

        public override void OnEggsLocated(EggBed eggBed)
        {
        }

        public override void OnFoodLocated(Foodbed foodBed)
        {
            if(!hungry) return;
            StateController.SetState(AIState.Follow, 
                followTarget: foodBed,
                onTargetReach: () =>
                {
                    DigOut(0f);
                    if (foodBed.Eat())
                    {
                        hungry = false;
                        StartCoroutine(HungerRoutine());
                    }
                    
                    StateController.SetState(AIState.Wander);
                },
                reachDistance: 1.3f);
        }

        protected override void DamageTaken()
        {
            InterruptDigging();
            DigIn(3f);
            AttackPlayer();
        }

        private void InterruptDigging()
        {
            if (diggingRoutine is null) return;
            StopCoroutine(diggingRoutine);
            diggingRoutine = null;
            animator.Play(scriptable.WalkAnimHash);
        }

        private void DigIn(float delay)
        {
            if(digged) return;
            if(digDelayRoutine is not null) StopCoroutine(digDelayRoutine);
            digDelayRoutine = StartCoroutine(DigInDelayRoutine(delay));
        }

        private void DigOut(float delay)
        {
            if(!digged) return;
            if(digDelayRoutine is not null) StopCoroutine(digDelayRoutine);
            digDelayRoutine = StartCoroutine(DigOutDelayRoutine(delay));
        }

        private IEnumerator DiggingInRoutine()
        {
            StateController.CancelCallback();
            StateController.TakeMoveControl();
            animator.Play(DiggingInAnimHash);
            yield return new WaitForSeconds(9 / 11f);
            minimapIcon.gameObject.SetActive(false);
            digged = true;
            diggingRoutine = null;
            StateController.SetEtherial(true);
            dirtParticles.Play();
            StateController.ReturnMoveControl();
            if (TimeManager.IsDay)
                StartCoroutine(FleeRoutine());
            else
            {
                StateController.SetState(AIState.Wander);
                DigOut(Random.Range(5f, 8f));
            }
        }

        private IEnumerator DiggingOutRoutine()
        {
            StateController.TakeMoveControl();
            animator.Play(DiggingOutAnimHash);
            dirtParticles.Stop();
            yield return new WaitForSeconds(2 / 11f);
            minimapIcon.gameObject.SetActive(true);
            digged = false;
            StateController.SetEtherial(false);
            yield return new WaitForSeconds(9 / 11f);
            StateController.ReturnMoveControl();
            diggingRoutine = null;
            if (TimeManager.IsDay)
                StartCoroutine(FleeRoutine());
            else
                DigIn(Random.Range(5f, 8f));
        }

        private IEnumerator DigInDelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            diggingRoutine = StartCoroutine(DiggingInRoutine());
            digDelayRoutine = null;
        }  
        
        private IEnumerator DigOutDelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            diggingRoutine = StartCoroutine(DiggingOutRoutine());
            digDelayRoutine = null;
        }

        protected override void OnDayStart(int day)
        {
            if(diggingRoutine is null) StartCoroutine(FleeRoutine());
        }
        
        private IEnumerator FleeRoutine()
        {
            StateController.SetEtherial(true);
            if(digDelayRoutine is not null) StopCoroutine(digDelayRoutine);
            DigIn(0f);
            yield return new WaitForSeconds(2f);
            dirtParticles.Stop();
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        
        private IEnumerator HungerRoutine()
        {
            yield return new WaitForSeconds(10);
            hungry = true;
        }
    }
}