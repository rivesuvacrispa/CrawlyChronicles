using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Food.Foodbeds;
using UnityEngine;
using Util;

namespace Gameplay.Food
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
                await UniTask.WaitUntil(() => foodbed.CanGrow, cancellationToken: cancellationToken);
                await UniTask.Delay(TimeSpan.FromSeconds(timeToGrow), cancellationToken: cancellationToken);
                
                foodbed.Grow();
            }
        }
    }
}