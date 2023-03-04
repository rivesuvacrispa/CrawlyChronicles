using System.Collections;
using Gameplay.AI;
using Gameplay.Food;
using Player;
using UnityEngine;
using Util;

namespace Gameplay.Enemies
{
    public class Cockroach : Enemy
    {
        [SerializeField] private float evadeDistance;
        [SerializeField] private float evadeSpeed;
        [SerializeField] private float evadeDuration;

        private Coroutine evadeRoutine;
        private bool canEvade = true;
        
        protected override void Start()
        {
            base.Start();
            AttackController.OnAttackStart += OnPlayerAttack;
        }

        public override void OnMapEntered() => stateController.SetState(AIState.Wander);

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed) { }

        public override void OnFoodLocated(FoodBed foodBed) { }

        protected override void OnDamageTaken()
        {
            AttackPlayer();
        }

        private void OnPlayerAttack()
        {
            var playerpos = Movement.Position;
            var direction = rb.position - playerpos;
            if(!canEvade || evadeRoutine is not null || direction.sqrMagnitude > evadeDistance ||
               !PhysicsUtility.AngleBetween(Manager.Instance.Transform.up, direction, 90)) return;
            StopAttack();
            rb.AddClampedForceBackwards(playerpos, 3, ForceMode2D.Impulse);
            evadeRoutine = StartCoroutine(EvadeRoutine(direction, 0.33f));
        }
        
        private IEnumerator EvadeRoutine(Vector2 direction, float duration)
        {
            WallCollider.enabled = true;
            stateController.SetState(AIState.None);
            float t = duration;
            while (t > 0)
            {
                stateController.TakeMoveControl();
                rb.RotateTowardsPosition(direction, 10);
                t -= Time.deltaTime;
                yield return null;
            }

            t = evadeDuration;
            while (t > 0)
            {
                stateController.TakeMoveControl();
                rb.AddClampedForceBackwards(Movement.Position, evadeSpeed, ForceMode2D.Force);
                rb.RotateTowardsPosition(rb.position + rb.velocity, 10);
                t -= Time.deltaTime;
                yield return null;
            }

            evadeRoutine = null;
            stateController.SetState(AIState.Wander);
            WallCollider.enabled = false;
            stateController.ReturnMoveControl();
            StartCoroutine(EvasionCooldown(1.5f));
        }

        private IEnumerator EvasionCooldown(float duration)
        {
            canEvade = false;
            yield return new WaitForSeconds(duration);
            canEvade = true;
        }

        public override void OnWallCollision()
        {
            CancelEvade();
            stateController.SetState(AIState.Wander);
            StartCoroutine(EvasionCooldown(5f));
        }

        private void CancelEvade()
        {
            if(evadeRoutine is null) return; 
            StopCoroutine(evadeRoutine);
            stateController.ReturnMoveControl();
            evadeRoutine = null;
            WallCollider.enabled = false;
        }

        protected override void OnDayStart(int day)
        {
            if(fearless) return;
            CancelEvade();
            base.OnDayStart(day);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AttackController.OnAttackStart -= OnPlayerAttack;
        }
    }
}