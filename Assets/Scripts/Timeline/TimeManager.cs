using System.Collections;
using Environment;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private GlobalLight globalLight;
        [SerializeField] private Text dayText;
        [SerializeField] private int dayDurationInSeconds = 60;
        [SerializeField] private int nightDurationInSeconds = 240;

        private int dayCounter;
        private int time;

        private int cycleDuration;
        
        

        private void Start()
        {
            dayCounter = 1;
            UpdateUI();
            cycleDuration = dayDurationInSeconds + nightDurationInSeconds;
            StartCoroutine(DayCycleRoutine());
        }

        private IEnumerator DayCycleRoutine()
        {
            while (time >= 0)
            {
                time++;

                // Day ends
                if (time == dayDurationInSeconds)
                    StartNight();
                // Night ends (cycle ends)
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
            globalLight.SetDay();
        }

        private void StartNight()
        {
            globalLight.SetNight();
        }

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