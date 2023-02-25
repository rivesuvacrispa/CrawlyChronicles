using System.Collections;
using UnityEngine;

namespace Util
{
    public class PauseTrigger : MonoBehaviour
    {
        public delegate void PauseTriggerEvent(bool isPaused);
        public static event PauseTriggerEvent OnPauseTriggered;
        
        private void OnEnable()
        {
            StartCoroutine(TriggerRoutine());
        }

        private IEnumerator TriggerRoutine()
        {
            const float updateRate = 1 / 30f;
            bool lastCheck = false;

            while (enabled)
            {
                bool paused = Time.timeScale == 0;
                if (lastCheck != paused)
                {
                    OnPauseTriggered?.Invoke(paused);
                    lastCheck = paused;
                }

                yield return new WaitForSecondsRealtime(updateRate);
            }
        }
    }
}