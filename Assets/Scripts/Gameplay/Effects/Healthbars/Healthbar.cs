using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Pooling;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Util.Interfaces;

namespace Gameplay.Effects.Healthbars
{
    public class Healthbar : Poolable
    {
        [SerializeField] private float height = 14;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image catchImage;
        [SerializeField] private float catchSpeed;

        private IDamageable target;
        private Tween currentTween;
        private float catchImageAlpha = 1f;
        private bool stopUpdate;

        
        
        public override void OnFirstInstantiated()
        {
            transform.SetParent(GlobalDefinitions.WorldCanvasTransform, true);
        }

        public void SetArgs(HealthbarArguments args)
        {
            target = args.target;
            target.OnProviderDestroy += OnDamageableDestroy;
            target.OnDamageTaken += OnTargetDamageTaken;
            target.OnDeath += OnTargetDeath;
            UpdateWidth();
        }

        public override bool OnTakenFromPool(object data)
        {
            if (data is not HealthbarArguments args) return false;
            
            SetArgs(args);
            stopUpdate = false;
            
            return base.OnTakenFromPool(data);
        }

        private void Start()
        {
            mainImage.fillAmount = 1f;
            catchImage.fillAmount = 1f;
            SetAlpha(0);
            enabled = false;
        }

        protected virtual void Update()
        {
            if (stopUpdate) return;
            UpdatePosition();
        }

        public void SetValue(float value)
        {
            mainImage.fillAmount = value;
            SetAlpha(1f);
            UpdatePosition();
            enabled = true;
            StartCatch(value);
        }

        private void UpdatePosition()
        {
            if (target is null) return;
            
            Vector3 pos = target.Transform.position;
            pos.z = 0;
            pos.y += target.HealthbarOffsetY;
            transform.localPosition = pos;
        }

        private void OnTargetDamageTaken(IDamageable damageable, float damage)
        {
            SetValue(Mathf.Clamp01(damageable.CurrentHealth / damageable.MaxHealth));
        }

        protected virtual void OnTargetDeath(IDamageable damageable)
        {
            stopUpdate = true;
            target.OnProviderDestroy -= OnDamageableDestroy;
            target.OnDamageTaken -= OnTargetDamageTaken;
            target.OnDeath -= OnTargetDeath;
            PoolTask(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask PoolTask(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => currentTween == null, cancellationToken: cancellationToken);
            Pool();
        }

        private void StartCatch(float finalValue)
        {
            SetCatchImageAlpha(1f);
            currentTween?.Kill();
            currentTween = catchImage.DOFillAmount(finalValue, catchSpeed).SetSpeedBased().OnComplete(() =>
            {
                currentTween = null;
                OnValueCatch(finalValue);
            });
        }
        
        protected virtual void OnValueCatch(float value)
        {
            StartFade(value <= float.Epsilon ? 1f : 3f);
        }
        
        protected void StartFade(float duration)
        {
            SetCatchImageAlpha(0f);
            currentTween?.Kill();
            currentTween = DOTween.To(GetAlpha, SetAlpha, 0f, duration).OnComplete(() =>
            {
                currentTween = null;
                SetAlpha(0f);
                enabled = false;
            });
        }
        
        private void SetAlpha(float alpha)
        {
            bgImage.color = bgImage.color.WithAlpha(alpha * 0.8f);
            mainImage.color = mainImage.color.WithAlpha(alpha);
            catchImage.color = catchImage.color.WithAlpha(alpha * catchImageAlpha);
        }
        
        private void SetCatchImageAlpha(float alpha)
        {
            catchImageAlpha = alpha;
            catchImage.color = catchImage.color.WithAlpha(alpha);
        }

        private float GetAlpha() => mainImage.color.a;

        private void OnDamageableDestroy(IDestructionEventProvider provider)
        {
            stopUpdate = true;
            currentTween?.Kill();
            target.OnProviderDestroy += OnDamageableDestroy;
            target.OnDamageTaken += OnTargetDamageTaken;
            target.OnDeath += OnTargetDeath;
            Pool();
        }
        
        protected void UpdateWidth() => 
            ((RectTransform) transform).sizeDelta = new Vector2(target.HealthbarWidth, height);

    }
}