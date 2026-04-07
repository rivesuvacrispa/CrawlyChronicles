using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Util.Components
{
    public class PopAnimator : MonoBehaviour
    {
        private const float ANIMATION_DURATION = 0.25f;

        
        public async UniTask PlayPopIn(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.75f))
                .Append(transform.DOScale(Vector3.one, ANIMATION_DURATION * 0.25f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }

        public async UniTask PlayPopOut(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.25f))
                .Append(transform.DOScale(Vector3.zero, ANIMATION_DURATION * 0.75f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }

        public async UniTask PlayPop(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 0.75f, ANIMATION_DURATION * 0.2f))
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.5f))
                .Append(transform.DOScale(Vector3.one, ANIMATION_DURATION * 0.3f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }
    }
}