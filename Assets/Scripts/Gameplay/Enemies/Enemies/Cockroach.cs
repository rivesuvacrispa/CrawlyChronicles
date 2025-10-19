using System.Collections;
using Gameplay.AI;
using Gameplay.Breeding;
using Gameplay.Food;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Gameplay.Enemies.Enemies
{
    public class Cockroach : Enemy, IWallCollisionListener
    {
        [SerializeField] private float evadeDistance;
        [SerializeField] private float evadeSpeed;
        [SerializeField] private float evadeDuration;

        private Coroutine evadeRoutine;
        private bool canEvade = true;



        protected override void Start()
        {
            base.Start();
            EntityWallCollider.enabled = false;
            AttackController.OnAttackStart += OnPlayerAttack;
        }

        public override void OnMapEntered() => StateController.SetState(AIState.Wander);

        public override void OnPlayerLocated()
        {
            AttackPlayer();
        }

        public override void OnEggsLocated(EggBed eggBed) { }

        public override void OnFoodLocated(Foodbed foodBed) { }

        protected override void OnDamageTaken()
        {
            AttackPlayer();
        }

        private void OnPlayerAttack()
        {
            var playerpos = PlayerMovement.Position;
            var direction = rb.position - playerpos;
            if(!canEvade || evadeRoutine is not null || direction.sqrMagnitude > evadeDistance ||
               !PhysicsUtility.AngleBetween(PlayerManager.Instance.Transform.up, direction, 90)) return;
            StopAttack();
            rb.AddClampedForceBackwards(playerpos, 3, ForceMode2D.Impulse);
            evadeRoutine = StartCoroutine(EvadeRoutine(direction, 0.33f));
        }
        
        private IEnumerator EvadeRoutine(Vector2 direction, float duration)
        {
            EntityWallCollider.enabled = true;
            StateController.SetState(AIState.None);
            float t = duration;
            while (t > 0)
            {
                StateController.TakeMoveControl();
                rb.RotateTowardsPosition(direction, 10);
                t -= Time.deltaTime;
                yield return null;
            }

            t = evadeDuration;
            while (t > 0)
            {
                StateController.TakeMoveControl();
                rb.AddClampedForceBackwards(PlayerMovement.Position, evadeSpeed, ForceMode2D.Force);
                rb.RotateTowardsPosition(rb.position + rb.linearVelocity, 10);
                t -= Time.deltaTime;
                yield return null;
            }

            AttackPlayer();
            EntityWallCollider.enabled = false;
            StateController.ReturnMoveControl();
            StartCoroutine(EvasionCooldown(1.5f));
            evadeRoutine = null;
        }

        private IEnumerator EvasionCooldown(float duration)
        {
            canEvade = false;
            yield return new WaitForSeconds(duration);
            canEvade = true;
        }
        
        private void CancelEvade()
        {
            if(evadeRoutine is null) return; 
            StopCoroutine(evadeRoutine);
            StateController.ReturnMoveControl();
            evadeRoutine = null;
            EntityWallCollider.enabled = false;
        }

        protected override void OnDayStart(int day)
        {
            CancelEvade();
            base.OnDayStart(day);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AttackController.OnAttackStart -= OnPlayerAttack;
        }
        
        
        // IWallCollisionListener
        public EntityWallCollider EntityWallCollider { get; set; }
        public void OnWallCollisionEnter()
        {
            CancelEvade();
            StateController.SetState(AIState.Wander);
            StartCoroutine(EvasionCooldown(5f));
        }

        public void OnWallCollisionExit() { }
    }
}