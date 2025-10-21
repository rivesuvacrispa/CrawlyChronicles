using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Bosses;
using Gameplay.Map;
using Scriptable;
using Timeline;
using UI.Menus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private List<Enemy> neutrals = new();
        [SerializeField] private List<Enemy> enemies = new();
        [SerializeField] private List<EnemyWave> waves = new();
        [Header("Spawn rates")]
        [SerializeField] private float neutralsPerMinute;
        [SerializeField] private float enemyPerMinute;
        [SerializeField] private float enemyPerDayModifier;

        private float currentEnemyPerMinute;
        private float currentEnemyPerDay;
        
        private static int enemyCounter = 0;

        private CancellationTokenSource cts = new();
        
        
        

        private void Start() => OnDifficultyChanged(SettingsMenu.SelectedDifficulty);

        private void OnEnable()
        {
            SubToEvents();
        }

        private void SpawnEnemy(Enemy toSpawn)
        {
            if(toSpawn is null || BossSpawner.BossAlive) return;
            
            Enemy enemy = Instantiate(toSpawn, MapManager.GameObjectsTransform);
            enemy.gameObject.name = $"{toSpawn.name}#{enemyCounter}";
            var spawnPoint = MapManager.GetRandomSpawnPoint();
            enemy.SpawnLocation = spawnPoint;
            enemy.transform.position = spawnPoint.SpawnPosition;
            enemyCounter++;
        }
        
        private async UniTask DayTask(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                SpawnEnemy(neutrals[Random.Range(0, neutrals.Count)]);
                await UniTask.Delay(TimeSpan.FromSeconds(60f / neutralsPerMinute), cancellationToken: cancellationToken);
            }
        }

        private async UniTask NightTask(int day, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
            
            float delay = 60f / (currentEnemyPerMinute + currentEnemyPerDay * (day - 1));
            // var wave = enemies.Where(enemy => enemy.Scriptable.CanSpawnSinceDay <= day)
            //     .OrderBy(_ => Random.value)
            //     .Take(Random.Range(2, 5))
            //     .ToArray();
            // var len = wave.Length;

            var wave = waves.Where(wave => wave.CanSpawnFromDay <= day)
                .OrderBy(_ => Random.value)
                .FirstOrDefault();
            
            if (wave is null) return;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                // var enemy = wave[Random.Range(0, len)];
                SpawnEnemy(wave.GetRandomEnemy());
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }
        }

        private void OnDayStart(int day)
        {
            DisposeTokenSource();
            cts = new CancellationTokenSource();
            DayTask(cts.Token).Forget();
        }

        private void OnNightStart(int day)
        {
            DisposeTokenSource();
            cts = new CancellationTokenSource();
            NightTask(day, cts.Token).Forget();
        }

        private void OnResetRequested()
        {
            cts.Cancel();
            OnDayStart(0);
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            float modifier = difficulty.EnemySpawnRate;
            currentEnemyPerMinute = enemyPerMinute * modifier;
            currentEnemyPerDay = enemyPerDayModifier * modifier;
        }
        
        private void SubToEvents()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
            TimeManager.OnDayStart += OnDayStart;
            TimeManager.OnNightStart += OnNightStart;
        }
        
        private void OnDisable()
        {
            UnsubFromEvents();
            DisposeTokenSource();
        }

        private void UnsubFromEvents()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
            TimeManager.OnDayStart -= OnDayStart;
            TimeManager.OnNightStart -= OnNightStart;
        }

        private void DisposeTokenSource()
        {
            if(cts is null) return;
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        private void OnDestroy()
        {
            UnsubFromEvents();
            DisposeTokenSource();
        }

    }
}