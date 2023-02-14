using System.Collections;
using Gameplay.Enemies;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image catchImage;
        [SerializeField] private float catchSpeed;

        private Transform target;
        private float offset;
        private Coroutine currentRoutine;



        public void SetTarget(Enemy enemy)
        {
            target = enemy.transform;
            offset = enemy.Scriptable.HealthbarOffsetY;
            SetWidth(enemy.Scriptable.HealthbarWidth);
        }
        
        public void SetValue(int health, float value)
        {
            mainImage.fillAmount = value;
            SetAlpha(1f);
            if(currentRoutine is not null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(CatchRoutine(health, value));
        }
        
        private void Start()
        {
            mainImage.fillAmount = 1f;
            catchImage.fillAmount = 1f;
            SetAlpha(0);
        }
        
        private void Update()
        {
            Vector3 pos = target.position;
            pos.z = 0;
            pos.y += offset;
            transform.localPosition = pos;
        }
        
        private IEnumerator CatchRoutine(int health, float finalValue)
        {
            while (catchImage.fillAmount - mainImage.fillAmount > 0.01f)
            {
                catchImage.fillAmount = Mathf.MoveTowards(catchImage.fillAmount, mainImage.fillAmount,
                    catchSpeed * Time.deltaTime);
                yield return null;
            }

            catchImage.fillAmount = finalValue;

            currentRoutine = StartCoroutine(health <= 0 ? DeathRoutine() : FadeRoutine());
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
            Destroy(gameObject);
        }

        private IEnumerator FadeRoutine()
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
        }

        private void SetAlpha(float alpha)
        {
            bgImage.color = bgImage.color.WithAlpha(alpha * 0.8f);
            mainImage.color = mainImage.color.WithAlpha(alpha);
            catchImage.color = catchImage.color.WithAlpha(alpha);
        }
        
        private void SetWidth(float width) => ((RectTransform) transform).sizeDelta = new Vector2(width, 14);

    }
}