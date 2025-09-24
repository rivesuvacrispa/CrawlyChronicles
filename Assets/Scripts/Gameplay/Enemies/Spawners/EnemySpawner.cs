using System.Collections;
using Definitions;
using Gameplay.Map;
using UI;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies.Spawners
{
    public class EnemySpawner : MonoBehaviour, INotificationProvider
    {
        [SerializeField] private Scriptable.EnemySpawner spawner;

        private int t;

        protected virtual void Start()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            GlobalDefinitions.CreateNotification(this, true, true);
        }

        private void OnEnable() => StartCoroutine(SpawnRoutine());

        private IEnumerator SpawnRoutine()
        {
            while (enabled)
            {
                t = spawner.SpawnTime;
                while (t > 0)
                {
                    OnDataUpdate?.Invoke();
                    yield return new WaitForSeconds(1f);
                    t--;
                }
                Spawn();
            }
        }

        private void Spawn()
        {
            var enemy = Instantiate(spawner.Enemy, MapManager.GameObjectsTransform);
            enemy.OnSpawnedBySpawner();
            enemy.transform.position = transform.position;
        }

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            OnProviderDestroy?.Invoke();
        }

        private void OnResetRequested() => Destroy(gameObject);

        

        // INotificationProvider
        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public string NotificationText => $"{t}s";
    }
}