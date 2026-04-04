using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util;

namespace Gameplay.Food.Foodbeds
{
    public class FoodbedRegrower : MonoBehaviour
    {
        [SerializeField] private Foodbed foodbed;
        [SerializeField, Range(1, 60)] private float timeToGrow;

        private void OnEnable()
        {
            GrowTask(gameObject.CreateCommonCancellationToken()).Forget();
        }

        private async UniTask GrowTask(CancellationToken cancellationToken)
        {
            while (isActiveAndEnabled)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToGrow), cancellationToken: cancellationToken);
                foodbed.Grow();
            }
        }
    }
}