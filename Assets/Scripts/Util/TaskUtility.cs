using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Util
{
    public static class TaskUtility
    {
        public static async UniTask MoveUntilFacingAndCloseEnough(
            Rigidbody2D actorRb,
            Transform target,
            float moveSpeed,
            float rotationSpeed, 
            float reachDistance,
            CancellationToken cancellationToken = default,
            Vector3 staticTarget = default)
        {
            if(staticTarget.Equals(default)) staticTarget = Vector3.zero;
            reachDistance *= reachDistance;
            float angle = float.MaxValue;
            float distance = float.MaxValue;
            while (angle >= 15 || distance > reachDistance)
            {
                Vector2 pos = actorRb.position;
                Vector2 targetPos = target is null ? staticTarget : target.position;
                Vector2 up = actorRb.transform.up;
                distance = (pos - targetPos).sqrMagnitude;
                
                angle = Vector2.Angle(up, targetPos - pos);
                actorRb.AddClampedForceTowards(pos + up, moveSpeed, ForceMode2D.Force);
                actorRb.RotateTowardsPosition(targetPos, rotationSpeed);
                await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);

            }
        }

        public static async UniTask WaitUntilDistanceReached(Rigidbody2D a, Transform b, float moveSpeed, float rotationSpeed, float reachDistance, CancellationToken cancellationToken = default)
        {
            reachDistance *= reachDistance;
            float distance = float.MaxValue;
            while (distance > reachDistance)
            {
                Vector2 aPos = a.position;
                Vector2 bPos = b.position;
                distance = (aPos - bPos).sqrMagnitude;
                a.AddClampedForceTowards(aPos + (Vector2) a.transform.up, moveSpeed, ForceMode2D.Force);
                a.RotateTowardsPosition(bPos, rotationSpeed);
                await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);
            }
        }

        public static async UniTask StepTowardsWhileReachingDistance(Rigidbody2D a, Transform b, float moveSpeed,
            float rotationSpeed, float reachDistance, CancellationToken cancellationToken = default)
        {
            a.AddClampedForceTowards(a.position + (Vector2) a.transform.up, moveSpeed, ForceMode2D.Force);
            a.RotateTowardsPosition(b.position, rotationSpeed);
            await WaitUntilDistanceReached(a, b, moveSpeed, rotationSpeed, reachDistance, cancellationToken);
        }
        
        public static async UniTask WaitUntilDistanceGained(Rigidbody2D a, Transform b, float moveSpeed, float rotationSpeed, float reachDistance, CancellationToken cancellationToken = default)
        {
            reachDistance *= reachDistance;
            float distance = float.MinValue;
            while (distance < reachDistance)
            {
                Vector2 aPos = a.position;
                Vector2 bPos = b.position;
                distance = (aPos - bPos).sqrMagnitude;
                a.AddClampedForceTowards(aPos + (Vector2) a.transform.up, moveSpeed, ForceMode2D.Force);
                a.RotateTowardsPosition(bPos, rotationSpeed);
                await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);

            }
        }

        public static async UniTask WaitUntilMovespeedIsLessThan(Rigidbody2D rb, float speed, CancellationToken cancellationToken = default)
        {
            float currentSpeed = float.MaxValue;
            while (currentSpeed > speed)
            {
                currentSpeed = rb.velocity.magnitude;
                await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);

            }
        }

        public static async UniTask MoveTowardsForAPeriodOfTime(Rigidbody2D rb, Vector3 target, float velocity, float period, CancellationToken cancellationToken = default)
        {
            float t = period;
            while (t > 0)
            {
                rb.AddClampedForceTowards(target, velocity, ForceMode2D.Force);
                t -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);
            }
        }
    }
}