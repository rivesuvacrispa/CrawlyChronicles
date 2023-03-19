using System.Collections;
using Definitions;
using Gameplay.AI;
using Gameplay.Food;
using UnityEngine;

namespace Gameplay.Enemies
{
    public class Antlion : Enemy
    {
        private Coroutine funnelingRoutine;
        
        private static readonly int Dig0AnimHash = Animator.StringToHash("AntlionDig0");
        private static readonly int Dig1AnimHash = Animator.StringToHash("AntlionDig1");
        private static readonly int Dig2AnimHash = Animator.StringToHash("AntlionDig2");
        private static readonly int Dig3AnimHash = Animator.StringToHash("AntlionDig3");
        private static readonly int DiggedAnimHash = Animator.StringToHash("AntlionDigged");
        private static readonly int TrapAnimHash = Animator.StringToHash("AntlionTrap");

        private SandFunnel funnel;
        
        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Follow, 
                onTargetReach: StartFunneling,
                reachDistance: 3.5f);
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
            StopFunneling();
        }

        private void StopFunneling()
        {
            if(funnelingRoutine is null) return;
            
            StopCoroutine(funnelingRoutine);
            if (funnel is not null) funnel.enabled = false;
            funnelingRoutine = null;
            animator.speed = 1;
            AttackPlayer();
        }
        
        private void StartFunneling()
        {
            if(funnelingRoutine is not null) return;
            
            funnelingRoutine = StartCoroutine(FunnelingRoutine(4f));
        }

        private IEnumerator FunnelingRoutine(float duration)
        {
            if(funnel is null) funnel = GlobalDefinitions.CreateSandFunnel(transform.position);
            stateController.SetState(AIState.None);
            stateController.TakeMoveControl();
            float stageDuraton = duration / 4;
            animator.Play(Dig0AnimHash);
            yield return new WaitForSeconds(stageDuraton);
            animator.Play(Dig1AnimHash);
            yield return new WaitForSeconds(stageDuraton);
            animator.Play(Dig2AnimHash);
            yield return new WaitForSeconds(stageDuraton);
            animator.speed = 1 / stageDuraton;
            animator.Play(Dig3AnimHash);
            yield return new WaitForSeconds(stageDuraton * 7 / 11f);
            stateController.SetEtherial(true);
            yield return new WaitForSeconds(stageDuraton * 4 / 11f);
            animator.speed = 1;
            animator.Play(DiggedAnimHash);
            yield return new WaitForSeconds(2f);
            animator.Play(TrapAnimHash);
            funnelingRoutine = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (funnel is not null) funnel.enabled = false;
        }
    }
}