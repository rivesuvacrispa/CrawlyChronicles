using System;
using System.Collections;
using Gameplay.AI;
using Gameplay.Food;
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
        private Coroutine digCooldownRoutine;
        
        
        public override void OnMapEntered() => DigIn(0.5f);

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
                followTarget: foodBed.gameObject,
                onTargetReach: o => {
                    if(o is not null)
                    {
                        DigOut(() =>
                        {
                            foodBed.Eat();
                            hungry = false;
                            StartCoroutine(HungerRoutine());
                        });
                    }
                    stateController.SetState(AIState.Wander);
                },
                reachDistance: 1.3f);
        }

        protected override void OnDamageTaken()
        {
            InterruptDigging();
            if(digCooldownRoutine is null) DigIn(5f);
            stateController.SetState(AIState.Follow, 
                onTargetReach: o => BasicAttack(),
                reachDistance: 0.75f);
        }

        private void InterruptDigging()
        {
            if (diggingRoutine is null) return;
            StopCoroutine(diggingRoutine);
            diggingRoutine = null;
            animator.Play(walkHash);
        }

        private void DigIn(float delay)
        {
            if(digged) return;
            digCooldownRoutine = StartCoroutine(DigCooldownRoutine(delay));
        }

        private void DigOut(Action continuation)
        {
            if(!digged) return;
            diggingRoutine = StartCoroutine(DiggingOutRoutine(continuation));
        }

        private IEnumerator DiggingInRoutine()
        {
            stateController.CancelCallback();
            stateController.TakeMoveControl();
            animator.Play(DiggingInAnimHash);
            yield return new WaitForSeconds(9 / 11f);
            digged = true;
            stateController.SetEtherial(true);
            dirtParticles.Play();
            stateController.ReturnMoveControl();
            stateController.SetState(AIState.Wander);
            diggingRoutine = null;
        }

        private IEnumerator DiggingOutRoutine(Action continuation)
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
            DigIn(Random.Range(4.5f, 6.5f));
            continuation();
        }

        private IEnumerator DigCooldownRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            digCooldownRoutine = null;
            diggingRoutine = StartCoroutine(DiggingInRoutine());
        }
        
        private IEnumerator HungerRoutine()
        {
            yield return new WaitForSeconds(10);
            hungry = true;
        }
    }
}