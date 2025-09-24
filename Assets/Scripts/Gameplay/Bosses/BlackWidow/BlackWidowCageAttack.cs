using System;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowCageAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject threadPrefab;
        [SerializeField] private BlackWidowAnimatorHanging animatorHanging;
        [SerializeField] private BlackWidowThreadCage cagePrefab;
        [Header("Attack Settings")]
        [SerializeField] private int threadsAmount = 16;
        [FormerlySerializedAs("cageWidth")] [SerializeField] private float cageRadius = 6f;
        [SerializeField] private float duration;

        private BlackWidowThreadCage cageInstance;
        
        private void OnEnable()
        {
            CreateCagePivot();
            CageAttackTask().Forget();
        }

        private void OnDisable()
        {
            if (cageInstance is not null) cageInstance.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (cageInstance is not null) Destroy(cageInstance);
        }

        private void CreateCagePivot()
        {
            if (cageInstance is not null) return;

            cageInstance = Instantiate(cagePrefab);
            cageInstance.SetTargetRadius(cageRadius);
        }

        private async UniTask CageAttackTask()
        {
            float anglePerThread = Mathf.PI * 2 / threadsAmount;
            float currentAngle = 0;
            TimeSpan threadDelay = TimeSpan.FromSeconds(duration / threadsAmount);
            
            for (int i = 0; i < threadsAmount; i++)
            {
                Vector3 threadOffset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * cageRadius;
                currentAngle += anglePerThread;
                
                Transform thread = Instantiate(threadPrefab, cageInstance.transform).transform;
                thread.localPosition = threadOffset;
                thread.localRotation = Quaternion.AngleAxis(currentAngle * Mathf.Rad2Deg, new Vector3(0, 0, 1));
                
                await UniTask.Delay(threadDelay);
            }
        }
    }
}