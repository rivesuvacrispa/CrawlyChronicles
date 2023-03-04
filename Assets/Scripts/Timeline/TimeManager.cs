using System.Collections;
using GameCycle;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [SerializeField] private Text dayText;
        [SerializeField] private Text lifespanText;
        [SerializeField] private int dayDurationInSeconds = 30;
        [SerializeField] private int nightDurationInSeconds = 120;
        [SerializeField] private int playerLifespanInSeconds = 300;
        [SerializeField] private ParticleSystem fireflyParticles;
        
        private int dayCounter;
        private int time;
        private int cycleDuration;
        private Coroutine lifespanRoutine;

        public delegate void DayCycleEvent(int dayCounter);
        public static event DayCycleEvent OnDayStart;
        public static event DayCycleEvent OnNightStart;

        public int DayCounter => dayCounter;

        public static bool IsDay => Instance.time < Instance.dayDurationInSeconds;
        
        private TimeManager() => Instance = this;

        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void Start()
        {
            StopAllCoroutines();
            dayCounter = 0;
            ResetLifespan();
            StartDay(dayDurationInSeconds / 2);
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

        private IEnumerator LifespanRoutine()
        {
            float timeLeft = playerLifespanInSeconds;
            while (timeLeft > 0)
            {
                lifespanText.text = $"{(int) timeLeft / 60:0}:{timeLeft % 60:00}";
                if(timeLeft <= 10) DeathCounter.StartCounter(timeLeft);
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            lifespanText.text = "0:00";
            Player.Manager.Instance.Die();
        }
        
        public void ResetLifespan()
        {
            if(lifespanRoutine is not null) StopCoroutine(lifespanRoutine);
            lifespanRoutine = StartCoroutine(LifespanRoutine());
            DeathCounter.StopCounter();
        }

        private void StartDay(int startFrom = 0)
        {
            fireflyParticles.Stop();
            StatRecorder.daysSurvived++;
            dayCounter++;
            time = startFrom;
            OnDayStart?.Invoke(dayCounter);
        }

        private void StartNight()
        {
            if(fireflyParticles.gameObject.activeSelf) fireflyParticles.Play();
            OnNightStart?.Invoke(dayCounter);
        }

        private void UpdateUI()
        {
            bool isDay = time < dayDurationInSeconds;
            string prefix = isDay ? "Day" : "Night";
            int timeLeft = isDay ? 
                dayDurationInSeconds - time :
                cycleDuration - time;
            dayText.text = $"{prefix} {dayCounter} - {timeLeft / 60}:{(timeLeft % 60):00}";
        }

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        private void OnResetRequested() => Start();
    }
}