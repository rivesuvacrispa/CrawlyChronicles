using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Util.Interfaces;

namespace UI.Elements
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private float height = 14;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image catchImage;
        [SerializeField] private float catchSpeed;

        private IDamageable target;
        private Tween currentTween;


        public void SetTarget(IDamageable damageable)
        {
            target = damageable;
            damageable.OnProviderDestroy += OnDamageableDestroy;
            UpdateWidth();
        }
        
        public void SetValue(float value)
        {
            mainImage.fillAmount = value;
            SetAlpha(1f);
            UpdatePosition();
            enabled = true;
            StartCatch(value);
        }
        
        private void Start()
        {
            mainImage.fillAmount = 1f;
            catchImage.fillAmount = 1f;
            SetAlpha(0);
            enabled = false;
        }

        protected virtual void Update() => UpdatePosition();

        private void UpdatePosition()
        {
            if (target is null) return;
            
            Vector3 pos = target.Transform.position;
            pos.z = 0;
            pos.y += target.HealthbarOffsetY;
            transform.localPosition = pos;
        }

        private void StartCatch(float finalValue)
        {
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
            catchImage.color = catchImage.color.WithAlpha(alpha);
        }

        private float GetAlpha() => mainImage.color.a;

        private void OnDamageableDestroy(IDestructionEventProvider provider)
        {
            currentTween?.Kill();
            provider.OnProviderDestroy -= OnDamageableDestroy;
            if (!Application.isEditor) Destroy(gameObject);
        }
        
        protected void UpdateWidth() => 
            ((RectTransform) transform).sizeDelta = new Vector2(target.HealthbarWidth, height);

    }
}