using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Pooling;
using TMPro;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Effects.DamageText
{
    public class DamageText : Poolable
    {
        [SerializeField] private TMP_Text text;

        private static DamageTextProperties defaultProperties;
        private float lifetime = 1.1f;

        private void Awake()
        {
            defaultProperties ??= new DamageTextProperties(new VertexGradient(Color.white), text.font, text.fontSize, lifetime);
        }

        private void ApplyProperties([NotNull] DamageTextProperties properties)
        {
            text.enableVertexGradient = properties.color is not null;
            text.colorGradient = properties.color ??  new VertexGradient(Color.white);
            text.font = properties.font ? properties.font : defaultProperties.font;
            text.fontSize = defaultProperties.size * properties.size;
            lifetime = defaultProperties.lifetime * properties.lifetime;
        }
        
        private void ApplyDefaultProperties()
        {
            text.enableVertexGradient = false;
            text.color = Color.white;
            text.font = defaultProperties.font;
            text.fontSize = defaultProperties.size;
            lifetime = defaultProperties.lifetime;
        }

        public override void OnFirstInstantiated()
        {
            transform.SetParent(GlobalDefinitions.WorldCanvasTransform, true);
        }

        public override bool OnTakenFromPool(object data)
        {
            if (data is not DamageTextArguments args) return false;

            if (args.damageInstance.source.owner is ICanChangeDamageText changeDamageText && 
                changeDamageText.ShouldChangeDamageText() && 
                changeDamageText.GetDamageTextProperties() is { })
            {
                ApplyProperties(changeDamageText.GetDamageTextProperties());
            } else 
                ApplyDefaultProperties();
            
            text.color = text.color.WithAlpha(1f);
            transform.position = args.position + (Vector3) Random.insideUnitCircle * 0.1f;
            text.SetText($"{args.damageInstance.Damage:0.##}");
            AnimationTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
            return base.OnTakenFromPool(data);
        }

        private async UniTask AnimationTask(CancellationToken cancellationToken)
        {
            await text.DOColor(text.color.WithAlpha(0f), lifetime)
                .SetEase(GlobalDefinitions.DamageTextEase)
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
            ((IPoolable)this).Pool();
        }
    }
}