using System;
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowStormAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject threadPrefab;
        [SerializeField] private BlackWidowAnimatorHanging animatorHanging;
        [Header("Attack Settings")]
        [SerializeField] private int minThreadsPerAttack = 3;
        [SerializeField] private int attackCount = 3;
        [SerializeField] private float threadDelaySeconds = 0.25f;
        [SerializeField] private float attacksDelaySeconds = 1.5f;
        private void OnEnable()
        {
            ThreadAttackTask().Forget();
        }

        private void OnDisable()
        {
            
        }

        private async UniTask ThreadAttackTask()
        {
            Transform playerTransform = PlayerManager.Instance.Transform;
            TimeSpan threadDelay = TimeSpan.FromSeconds(threadDelaySeconds);
            TimeSpan attacksDelay = TimeSpan.FromSeconds(attacksDelaySeconds);
            
            for (int i = 0; i < attackCount; i++)
            {
                animatorHanging.SetRandomHangPivot();

                int threadsCount = minThreadsPerAttack + Random.Range(0, 2);
                for (int t = 0; t < threadsCount; t++)
                {
                    Transform thread = Instantiate(threadPrefab).transform;
                    thread.position = playerTransform.position + (Vector3) Random.insideUnitCircle * 0.25f;
                    thread.rotation = Quaternion.Euler(0, 0, Random.value * 360);
                    await UniTask.Delay(threadDelay);
                }

                await UniTask.Delay(attacksDelay);
            }

            enabled = false;
        }
    }
}