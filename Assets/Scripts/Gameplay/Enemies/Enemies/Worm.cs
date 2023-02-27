using System.Collections;
using Gameplay.AI;
using Gameplay.Food;
using Timeline;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
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

        public override void OnFoodLocated(FoodBed foodBed)
        {
            if(!hungry) return;
            stateController.SetState(AIState.Follow, 
                followTarget: foodBed,
                onTargetReach: () =>
                {
                    DigOut(0f);
                    foodBed.Eat();
                    hungry = false;
                    StartCoroutine(HungerRoutine());
                    stateController.SetState(AIState.Wander);
                },
                reachDistance: 1.3f);
        }

        protected override void OnDamageTaken()
        {
            InterruptDigging();
            DigIn(3f);
            stateController.SetState(AIState.Follow, 
                onTargetReach: BasicAttack,
                reachDistance: 0.75f);
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
            stateController.CancelCallback();
            stateController.TakeMoveControl();
            animator.Play(DiggingInAnimHash);
            yield return new WaitForSeconds(9 / 11f);
            digged = true;
            diggingRoutine = null;
            stateController.SetEtherial(true);
            dirtParticles.Play();
            stateController.ReturnMoveControl();
            if (TimeManager.IsDay)
                StartCoroutine(FleeRoutine());
            else
            {
                stateController.SetState(AIState.Wander);
                DigOut(Random.Range(5f, 8f));
            }
        }

        private IEnumerator DiggingOutRoutine()
        {
            stateController.TakeMoveControl();
            animator.Play(DiggingOutAnimHash);
            dirtParticles.Stop();
            yield return new WaitForSeconds(2 / 11f);
            digged = false;
            stateController.SetEtherial(false);
            yield return new WaitForSeconds(9 / 11f);
            stateController.ReturnMoveControl();
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
            stateController.SetEtherial(true);
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