using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Components;
using Util.Interfaces;

namespace Hitboxes
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHitbox : MonoBehaviour, IDamageableHitbox
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private Gradient immunityGradient;

        private int ImmuneSourceFromDamage => HashCode.Combine(GetHashCode(), 0);
        private int ImmuneSourceFromEnabled => HashCode.Combine(GetHashCode(), 1);

        public static readonly MultiSourceState Immune = new();
        private CancellationTokenSource cancelOnClearTokenSource;


        private void OnEnable()
        {
            cancelOnClearTokenSource = new CancellationTokenSource();
            PlayerManager.OnPlayerRespawned += OnPlayerRespawn;
        }

        private void OnDisable() => PlayerManager.OnPlayerRespawned -= OnPlayerRespawn;

        private void OnPlayerRespawn()
        {
            Clear();
        }


        public void Hit(DamageInstance instance) =>
            ImmunityTask(gameObject.CreateCommonCancellationToken(
                cancelOnClearTokenSource.Token)).Forget();

        public void Die()
        {
            Dead = true;
        }

        public bool Dead { get; private set; }
        public bool ImmuneToSource(DamageSource source) => Dead || Immune.State;

        private async UniTask ImmunityTask(CancellationToken cancellationToken)
        {
            float duration = PlayerManager.PlayerStats.ImmunityDuration;
            bodyPainter.Paint(immunityGradient, duration);

            Immune.Vote(ImmuneSourceFromDamage);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            Immune.Unvote(ImmuneSourceFromDamage);
        }

        public void Enable()
        {
            Immune.Unvote(ImmuneSourceFromEnabled);
        }

        public void Disable()
        {
            Immune.Vote(ImmuneSourceFromEnabled);
        }


        private void Clear()
        {
            Dead = false;
            cancelOnClearTokenSource?.Cancel();
            cancelOnClearTokenSource?.Dispose();
            cancelOnClearTokenSource = new CancellationTokenSource();
            Immune.Clear();
        }
    }
}