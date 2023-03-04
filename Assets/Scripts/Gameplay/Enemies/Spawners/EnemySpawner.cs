using System.Collections;
using Definitions;
using Gameplay.AI;
using Timeline;
using UnityEngine;
using Util;

namespace Gameplay.Enemies.Spawners
{
    public class EnemySpawner : MonoBehaviour, INotificationProvider
    {
        [SerializeField] private Scriptable.EnemySpawner spawner;

        private int t;

        protected virtual void Start()
        {
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
            var enemy = Instantiate(spawner.Enemy, GlobalDefinitions.GameObjectsTransform);
            enemy.OnSpawnedBySpawner();
            enemy.transform.position = transform.position;
        }

        private void OnDestroy() => OnProviderDestroy?.Invoke();


        public event IDestructionEventProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public string NotificationText => $"{t}s";
    }
}