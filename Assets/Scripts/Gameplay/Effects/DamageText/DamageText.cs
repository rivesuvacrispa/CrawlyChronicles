using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Pooling;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.Effects.DamageText
{
    public class DamageText : Poolable
    {
        [SerializeField] private TMP_Text text;

        
        
        public override void OnFirstInstantiated()
        {
            transform.SetParent(GlobalDefinitions.WorldCanvasTransform, true);
        }

        public override bool OnTakenFromPool(object data)
        {
            if (data is not DamageTextArguments args) return false;

            text.color = text.color.WithAlpha(1f);
            transform.position = args.position;
            text.SetText($"{args.damage:0.00}");
            AnimationTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            return base.OnTakenFromPool(data);
        }

        private async UniTask AnimationTask(CancellationToken cancellationToken)
        {
            await text.DOColor(text.color.WithAlpha(0f), 1.1f)
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
            Pool();
        }
    }
}