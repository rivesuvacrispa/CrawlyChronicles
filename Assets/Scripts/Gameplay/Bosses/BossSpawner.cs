using System.Collections;
using System.Collections.Generic;
using Definitions;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Scripts.Gameplay.Bosses
{
    public class BossSpawner : MonoBehaviour
    {
        [SerializeField] private List<Boss> bosses;
        [SerializeField] private int spawnInterval;
        [SerializeField] private Text encounterText;

        public static int BossesAmount { get; private set; }
        public static int SpawnInterval { get; private set; }

        private static Boss currentBoss;
        public static bool BossAlive => currentBoss is not null;
        
        
        private void Awake()
        {
            BossesAmount = bosses.Count;
            SpawnInterval = spawnInterval;
        }

        private void SpawnBoss(Boss boss)
        {
            if (currentBoss is not null)
            {
                currentBoss.Flee();
            }

            if (SettingsMenu.SelectedDifficulty.OverallDifficulty is OverallDifficulty.Peaceful)
                TimeManager.Instance.enabled = false;
            currentBoss = Instantiate(boss);
            currentBoss.SetLocation(GlobalDefinitions.GetRandomPointAroundMap(15));
            currentBoss.OnProviderDestroy += OnProviderDestroy;
        }
        
        private void OnNightStart(int night)
        {
            if(night % spawnInterval != 0) return;
            SpawnBoss(bosses[night / spawnInterval - 1]);
        }

        private void OnDayStart(int day)
        {
            if (day > BossesAmount * spawnInterval)
            {
                enabled = false;
                return;
            }

            if (day % spawnInterval == 0)
                StartCoroutine(EncounterRoutine(bosses[day / spawnInterval - 1].Scriptable.EncounterTitle));
        }

        private IEnumerator EncounterRoutine(string title)
        {
            yield return new WaitForSeconds(TimeManager.DayDuration * 5 / 6f * (TimeManager.DayCounter == 1 ? 0.5f : 1f));
            encounterText.text = title;
            encounterText.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            TimeManager.OnNightStart += OnNightStart;
            TimeManager.OnDayStart += OnDayStart;
        }

        private void OnDisable()
        {
            TimeManager.OnNightStart -= OnNightStart;
            TimeManager.OnDayStart -= OnDayStart;
        }

        private void OnDestroy()
        {
            TimeManager.OnNightStart -= OnNightStart;
            TimeManager.OnDayStart -= OnDayStart;
            OnProviderDestroy();
        }

        private void OnProviderDestroy()
        {
            if(currentBoss is null) return;
            if (SettingsMenu.SelectedDifficulty.OverallDifficulty is OverallDifficulty.Peaceful)
                TimeManager.Instance.StartDay();
            currentBoss.OnProviderDestroy -= OnProviderDestroy;
            currentBoss = null;
        }
    }
}