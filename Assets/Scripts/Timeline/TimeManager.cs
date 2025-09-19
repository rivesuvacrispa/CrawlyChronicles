using System.Collections;
using GameCycle;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Timeline
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [SerializeField] private LocalizedString localizedString;
        [SerializeField] private Text dayText;
        [SerializeField] private Text lifespanText;
        [SerializeField] private int dayDurationInSeconds = 30;
        [SerializeField] private int nightDurationInSeconds = 120;
        [SerializeField] private int playerLifespanInSeconds = 300;
        [SerializeField] private ParticleSystem fireflyParticles;

        private float lifetime;
        private int dayCounter;
        private int time;
        private int cycleDuration;
        private Coroutine lifespanRoutine;
        private Coroutine timeRoutine;

        public delegate void DayCycleEvent(int dayCounter);
        public static event DayCycleEvent OnDayStart;
        public static event DayCycleEvent OnNightStart;

        public static int DayCounter => Instance.dayCounter;

        public static bool IsDay => Instance.time < Instance.dayDurationInSeconds;
        public static int DayDuration => Instance.dayDurationInSeconds;
        
        private TimeManager() => Instance = this;

        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            localizedString.Arguments = new object[] { 1, 0, string.Empty};
            localizedString.StringChanged += UpdateText;
        }

        private void Start()
        {
            OnDisable();
            dayCounter = 0;
            ResetLifespan();
            cycleDuration = dayDurationInSeconds + nightDurationInSeconds;
            StartDay(dayDurationInSeconds / 2);
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

        private IEnumerator LifespanRoutine(float startingPoint)
        {
            lifetime = startingPoint;
            while (lifetime > 0)
            {
                lifespanText.text = $"{(int) lifetime / 60:0}:{lifetime % 60:00}";
                if(lifetime <= 10) DeathCounter.StartCounter(lifetime);
                lifetime -= Time.deltaTime;
                yield return null;
            }

            lifespanText.text = "0:00";
            Player.PlayerManager.Instance.Die(false);
        }
        
        public void ResetLifespan()
        {
            if(lifespanRoutine is not null) StopCoroutine(lifespanRoutine);
            lifespanRoutine = StartCoroutine(LifespanRoutine(playerLifespanInSeconds));
            DeathCounter.StopCounter();
        }

        public void StartDay(int startFrom = 0)
        {
            fireflyParticles.Stop();
            StatRecorder.daysSurvived++;
            dayCounter++;
            time = startFrom;
            OnDayStart?.Invoke(dayCounter);
            
            if (timeRoutine is null)
            {
                timeRoutine = StartCoroutine(DayCycleRoutine());
                enabled = true;
            }

            if (lifespanRoutine is null) 
                lifespanRoutine = StartCoroutine(LifespanRoutine(lifetime));
            UpdateUI();
        }

        private void StartNight()
        {
            if(fireflyParticles.gameObject.activeSelf) fireflyParticles.Play();
            OnNightStart?.Invoke(dayCounter);
        }

        private void UpdateUI()
        {
            bool isDay = time < dayDurationInSeconds;
            int timeLeft = isDay ? 
                dayDurationInSeconds - time :
                cycleDuration - time;

            localizedString.Arguments[0] = isDay ? 1 : 2;
            localizedString.Arguments[1] = dayCounter;
            localizedString.Arguments[2] = $"{timeLeft / 60}:{timeLeft % 60:00}";
            localizedString.RefreshString();
        }

        private void UpdateText(string text) => dayText.text = text;

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            localizedString.StringChanged -= UpdateText;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            timeRoutine = null;
            lifespanRoutine = null;
        }

        private void OnResetRequested()
        {
            enabled = true;
            Start();
        }
    }
}