using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.LilHorror
{
    public class LilHorrorHead : MonoBehaviour
    {
        private IUnitTarget currentTarget;
        private LilHorrorPart part;
        private bool Wandering => PlayerManager.Instance.Equals(currentTarget);

        
        private void Awake()
        {
            part = GetComponent<LilHorrorPart>();
        }

        private void OnEnable()
        {
            MoveTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            IDamageable.OnDamageTakenGlobal += OnDamageTakenGlobal;
            SetTarget(PlayerManager.Instance);
        }

        private void OnDisable()
        {
            IDamageable.OnDamageTakenGlobal -= OnDamageTakenGlobal;
        }

        private void OnDamageTakenGlobal(IDamageable damageable, DamageInstance instance)
        {
            if (damageable is not IUnitTarget t) return;
            
            if ((Wandering || currentTarget is null) &&
                damageable.Equals(PlayerManager.Instance) && 
                instance.source.owner is IUnitTarget { CanAggroUnit: true } attacker)
            {
                SetTarget(attacker);
                return;
            }
            
            if (t.CanAggroUnit)
                SetTarget(t);
        }

        private async UniTask MoveTask(CancellationToken cancellationToken)
        {
            while (isActiveAndEnabled)
            {
                IUnitTarget targetBefore = currentTarget;
                Vector3 destination = GetDestination();
                float distance = 0f;
                do
                {
                    float speedModifier = Wandering ? 0.5f : 1f;
                    distance = part.Move(destination, speedModifier);
                    await UniTask.NextFrame(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    
                    if (!(currentTarget?.Equals(targetBefore) ?? false))
                        break;
                    
                } while (distance >= 0.16f);
            }
        }

        private Vector3 GetDestination()
        {
            Vector3 currentPos = transform.position;
            Vector3 playerPos = PlayerPhysicsBody.Position;
            Vector3 wanderPos = Random.insideUnitCircle.normalized;
            
            if (Wandering)
                return playerPos + wanderPos * 2f;

            if (currentTarget is not null)
                return currentTarget.Transform.position;

            return currentPos + wanderPos;
        }

        private void SetTarget(IUnitTarget t)
        {
            if (currentTarget is not null) 
                currentTarget.OnUnitDetach -= OnUnitDetach;

            currentTarget = t;
            if (currentTarget is not null)
                currentTarget.OnUnitDetach += OnUnitDetach;
        }

        private void OnUnitDetach()
        {
            SetTarget(PlayerManager.Instance);
        }
    }
}