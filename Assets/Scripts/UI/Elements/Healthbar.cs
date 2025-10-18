using System.Collections;
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
        protected Coroutine currentRoutine;


        public void SetTarget(IDamageable damageable)
        {
            target = damageable;
            damageable.OnProviderDestroy += OnDamageableDestroy;
            UpdateWidth();
        }
        
        public void SetValue(float value)
        {
            Debug.Log($"Healthbar value: {value}");
            mainImage.fillAmount = value;
            SetAlpha(1f);
            UpdatePosition();
            enabled = true;
            if(currentRoutine is not null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(CatchRoutine(value));
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
        
        private IEnumerator CatchRoutine(float finalValue)
        {
            while (Mathf.Abs(catchImage.fillAmount - mainImage.fillAmount) > 0.01f)
            {
                catchImage.fillAmount = Mathf.MoveTowards(catchImage.fillAmount, mainImage.fillAmount,
                    catchSpeed * Time.deltaTime);
                yield return null;
            }

            catchImage.fillAmount = finalValue;

            OnValueCatched(finalValue);
        }

        protected virtual void OnValueCatched(float value)
        {
            currentRoutine = StartCoroutine(value <= float.Epsilon ? DeathRoutine() : FadeRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            float t = 0;
            while (t < 1f)
            {
                SetAlpha(1 - t);   
                t += Time.deltaTime;
                yield return null;
            }
            SetAlpha(0);
            enabled = false;
        }

        protected IEnumerator FadeRoutine()
        {
            float t = 0;
            yield return new WaitForSeconds(2f);
            while (t < 1f)
            {
                SetAlpha(1 - t);
                t += Time.deltaTime;
                yield return null;
            }
            SetAlpha(0f);
            enabled = false;
        }

        private void SetAlpha(float alpha)
        {
            bgImage.color = bgImage.color.WithAlpha(alpha * 0.8f);
            mainImage.color = mainImage.color.WithAlpha(alpha);
            catchImage.color = catchImage.color.WithAlpha(alpha);
        }

        private void OnDamageableDestroy(IDestructionEventProvider provider)
        {
            provider.OnProviderDestroy -= OnDamageableDestroy;
            if (!Application.isEditor) Destroy(gameObject);
        }
        
        protected void UpdateWidth() => 
            ((RectTransform) transform).sizeDelta = new Vector2(target.HealthbarWidth, height);

    }
}