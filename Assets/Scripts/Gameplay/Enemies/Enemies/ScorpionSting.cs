using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Player;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Enemies
{
    public class ScorpionSting : MonoBehaviour
    {
        [SerializeField] private GameObject attackGO;
        
        public bool IsAttacking { get; private set; }
        public Vector3 LatestDirection { get; private set; }
        public bool TrackPlayer { get; set; }
        public float CachedDistance { get; private set; }

        
        
        private void FixedUpdate()
        {
            if (!TrackPlayer) return;

            CachedDistance = Vector2.Distance(transform.position, PlayerManager.Instance.transform.position);
            if (CachedDistance > .5f)
                UpdateStingPosition();
        }

        private void UpdateStingPosition()
        {
            Vector3 direction = GetDirectionTowardsPlayer();
                
            if (!IsAttacking)
                transform.localPosition = direction.normalized * 0.45f;
            
            transform.rotation = Quaternion.AngleAxis(((Vector2)direction).GetAngle(), Vector3.forward);
        }
        
        public async UniTask AttackTask(CancellationToken cancellationToken)
        {
            IsAttacking = true;
            attackGO.SetActive(true);

            await DOTween.Sequence()
                .Append(transform.DOMove(transform.position + GetDirectionTowardsPlayer().normalized, 0.1f).OnComplete(
                    () =>
                    {
                        attackGO.SetActive(false);
                    }))
                .Append(transform.DOLocalMove(GetDirectionTowardsPlayer().normalized * 0.45f, 0.5f))
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
            
            attackGO.SetActive(false);
            IsAttacking = false;
        }

        private Vector3 GetDirectionTowardsPlayer()
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = PlayerManager.Instance.Transform.position;
            LatestDirection = targetPos - currentPos;
            return LatestDirection;
        }
    }
}