using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Util.Interfaces;

namespace UI.Elements
{
    public class DPSCounter : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 3)] private float updateInterval = 3f;
        [SerializeField] private TMP_Text dpsText;

        private float damageDoneSinceLastTick;
        private float targetDPS;
        private float currentDPS;
        private CancellationTokenSource cts;

        private void OnEnable()
        {
            IDamageable.OnDamageTakenGlobal += OnDamageTakenGlobal;

            cts = new CancellationTokenSource();
            UpdateTask(cts.Token).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnDisable()
        {
            IDamageable.OnDamageTakenGlobal -= OnDamageTakenGlobal;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        private void Update()
        {
            currentDPS = Mathf.MoveTowards(currentDPS, targetDPS, Mathf.Abs(targetDPS - currentDPS) * 0.1f);
            dpsText.text = $"{currentDPS:0.00} DPS";
        }

        private void OnDamageTakenGlobal(IDamageable damageable, DamageInstance instance)
        {
            if (damageable is not IDamageableEnemy) return;
            
            damageDoneSinceLastTick += instance.Damage;
        }

        private async UniTask UpdateTask(CancellationToken cancellationToken)
        {
            while (enabled)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(updateInterval), 
                    cancellationToken: cancellationToken);
                targetDPS = damageDoneSinceLastTick / updateInterval;
                damageDoneSinceLastTick = 0f;
            }
        }
    }
}