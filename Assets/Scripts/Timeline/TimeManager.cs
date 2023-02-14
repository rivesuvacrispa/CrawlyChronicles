using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private Text dayText;
        [SerializeField] private int dayDurationInSeconds = 60;
        [SerializeField] private int nightDurationInSeconds = 240;
        
        private int dayCounter;
        private int time;
        private int cycleDuration;

        public delegate void DayCycleEvent(int dayCounter);
        public static event DayCycleEvent OnDayStart;
        public static event DayCycleEvent OnNightStart;
        

        private void Start()
        {
            StartDay();
            UpdateUI();
            cycleDuration = dayDurationInSeconds + nightDurationInSeconds;
            StartCoroutine(DayCycleRoutine());
        }

        private IEnumerator DayCycleRoutine()
        {
            while (time >= 0)
            {
                time++;

                if (time == dayDurationInSeconds)
                    StartNight();
                else if (time == cycleDuration) 
                    StartDay();
                
                UpdateUI();

                yield return new WaitForSeconds(1f);
            }
        }

        private void StartDay()
        {
            dayCounter++;
            time = 0;
            OnDayStart?.Invoke(dayCounter);
        }

        private void StartNight() => OnNightStart?.Invoke(dayCounter);

        private void UpdateUI()
        {
            bool isDay = time < dayDurationInSeconds;
            string prefix = isDay ? "Day" : "Night";
            int timeLeft = isDay ? 
                dayDurationInSeconds - time :
                cycleDuration - time;
            dayText.text = $"{prefix} {dayCounter} - {timeLeft / 60}:{timeLeft % 60}";
        }
    }
}