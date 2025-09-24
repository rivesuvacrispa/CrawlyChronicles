using System.Collections;
using Definitions;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Map;
using UnityEngine;

namespace Gameplay.Enemies.Enemies
{
    public class DiggingWasp : SidefacedEnemy
    {
        [SerializeField] private ParticleSystem dirtParticles;
        
        private static readonly int DiggingAnimHash = Animator.StringToHash("DiggingWaspDigging");

        public new Scriptable.Enemies.DiggingWasp Scriptable
            => (Scriptable.Enemies.DiggingWasp) scriptable;

        private Coroutine diggingRoutine;
        private MaterialPropertyBlock propertyBlock;
        private static readonly int AggroPropID = Shader.PropertyToID("_Aggro");



        protected override void Start()
        {
            base.Start();
            propertyBlock = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(propertyBlock);
            if(spawnedBySpawner) SetAggro(true);
        }

        public override void OnMapEntered()
        {
            stateController.SetState(AIState.Wander);
            StartCoroutine(DiggingCooldown(Scriptable.DiggingCooldown));
        }

        public override void OnPlayerLocated()
        {
            if(spawnedBySpawner) AttackPlayer();
        }

        protected override void AttackPlayer(float reachDistance = 0)
        {
            base.AttackPlayer(reachDistance);
            SetAggro(true);
        }

        protected override void OnDamageTaken()
        {
            CancelDigging();
            AttackPlayer();
        }

        private IEnumerator DiggingCooldown(float duration)
        {
            while (enabled)
            {
                yield return new WaitForSeconds(duration * Random.Range(0.8f, 1.2f));
                if(diggingRoutine is null && stateController.CurrentState == AIState.Wander)
                    diggingRoutine = StartCoroutine(DiggingRoutine());
                yield return new WaitForSeconds(Scriptable.DiggingTime);
                yield return new WaitUntil(() => stateController.CurrentState == AIState.Wander);
            }
        }

        private IEnumerator DiggingRoutine()
        {
            dirtParticles.Play();
            stateController.SetState(AIState.None);
            animator.Play(DiggingAnimHash);
            yield return new WaitForSeconds(Scriptable.DiggingTime);
            var eggbed = Instantiate(Scriptable.EggBedPrefab, MapManager.GameObjectsTransform);
            eggbed.transform.position = rb.position;
            stateController.SetState(AIState.Wander);
            
            CancelDigging();
        }

        private void CancelDigging()
        {
            if(diggingRoutine is null) return;
            StopCoroutine(diggingRoutine);
            diggingRoutine = null;
            PlayCrawl();
            dirtParticles.Stop();
        }

        protected override void OnDayStart(int day)
        {
            StopCoroutine(nameof(DiggingCooldown));
            base.OnDayStart(day);
        }

        private void SetAggro(bool isAggressive)
        {
            propertyBlock.SetFloat(AggroPropID, isAggressive ? 1 : 0);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }

        public override void OnEggsLocated(EggBed eggBed) { }
        public override void OnFoodLocated(Foodbed foodBed) { }
    }
}