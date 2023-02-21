using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
    public class DeathCounter : MonoBehaviour
    {
        private static DeathCounter instance;

        [SerializeField] private Text numText;
        [SerializeField] private Animator animator;

        private readonly int animHash = Animator.StringToHash("DeathCounterPop");
        private Coroutine routine;
        
        
        
        private DeathCounter() => instance = this;
        
        public static void StartCounter(float timeLeft) => instance.StartCounterNonStatic(timeLeft);

        public static void StopCounter() => instance.StopCounterNonStatic();
        
        private void StartCounterNonStatic(float timeLeft)
        {
            if(routine is not null) return;
            gameObject.SetActive(true);
            routine = StartCoroutine(CounterRoutine(timeLeft));
        }

        private void StopCounterNonStatic()
        {
            if(routine is not null) StopCoroutine(routine);
            gameObject.SetActive(false);
            routine = null;
        }

        private IEnumerator CounterRoutine(float timeLeft)
        {
            while (timeLeft > 0)
            {
                numText.text = Mathf.CeilToInt(timeLeft).ToString();
                animator.Play(animHash);
                timeLeft--;
                yield return new WaitForSeconds(1f);
            }

            routine = null;
            gameObject.SetActive(false);
        }
    }
}