using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Effects.PlayerWeb;
using Gameplay.Mutations.EntityEffects.Slow;
using Gameplay.Mutations.EntityEffects.Stat;
using Gameplay.Player;
using Pooling;
using UI.Menus;
using UnityEngine;
using Util;

namespace Gameplay.Mutations.Active
{
    public class SpinneretGlands : ActiveAbility
    {
        [SerializeField] private ParticleSystem particles;

        [Header("Maximum Simultaneous Web")] 
        [SerializeField, Range(1, 20)] private int maxAmountLvl1;
        [SerializeField, Range(1, 20)] private int maxAmountLvl10;
        [Header("Movement Slow")] 
        [SerializeField, Range(0f, 1f)] private float movementSlowLvl1;
        [SerializeField, Range(0f, 1f)] private float movementSlowLvl10;
        [Header("Rotation Slow")] 
        [SerializeField, Range(0f, 1f)] private float rotationSlowLvl1;
        [SerializeField, Range(0f, 1f)] private float rotationSlowLvl10;
        [Header("Movement Speed")] 
        [SerializeField, Range(0f, 10f)] private float movementSpeedLvl1;
        [SerializeField, Range(0f, 10f)] private float movementSpeedLvl10;
        [Header("Rotation Speed")] 
        [SerializeField, Range(0f, 10f)] private float rotationSpeedLvl1;
        [SerializeField, Range(0f, 10f)] private float rotationSpeedLvl10;

        private ParticleSystem.MainModule mainModule;
        private ParticleSystem.ShapeModule shapeModule;
        private readonly Queue<PlayerWeb> webQueue = new();

        private int maxAmount;
        private float movementSlow;
        private float rotationSlow;
        private float movementSpeed;
        private float rotationSpeed;
        public static SlowEffectData DebuffEffectData { get; private set; }
        public static StatEffectData BuffEffectData { get; private set; }


        
        protected override void Awake()
        {
            base.Awake();
            mainModule = particles.main;
            shapeModule = particles.shape;
            MainMenu.OnResetRequested += OnResetRequested;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        private void OnResetRequested() => webQueue.Clear();

        public override bool CanActivate()
        {
            if (!base.CanActivate()) return false;

            return webQueue.All(w => !(Vector2.Distance(w.transform.position, MainCamera.WorldMousePos) < 1.6f));
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            maxAmount = LerpLevel(maxAmountLvl1, maxAmountLvl10, lvl);
            movementSlow = LerpLevel(movementSlowLvl1, movementSlowLvl10, lvl);
            rotationSlow = LerpLevel(rotationSlowLvl1, rotationSlowLvl10, lvl);
            movementSpeed = LerpLevel(movementSpeedLvl1, movementSpeedLvl10, lvl);
            rotationSpeed = LerpLevel(rotationSlowLvl1, rotationSpeedLvl10, lvl);

            DebuffEffectData = new SlowEffectData(1, movementSlow, rotationSlow);
            BuffEffectData = new StatEffectData(1, new PlayerStats(
                movementSpeed: movementSpeed, 
                rotationSpeed: rotationSpeed)
            );
            
            if (Application.IsPlaying(this) && isActiveAndEnabled)
            {
                RemoveExcessWeb(maxAmount);
            }
        }

        private void RemoveExcessWeb(int max)
        {
            int queueLength = webQueue.Count;
            if (queueLength <= max) return;

            int amountToRemove = queueLength - max;
            for (int i = 0; i < amountToRemove; i++)
            {
                if (webQueue.TryDequeue(out PlayerWeb web))
                {
                    web.PoolWithAnimation();
                }
            }
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            AttackController.CancelAttack();
            PlayerMovement.CancelKnockback();
                
            ActivateTask(CancellationTokenSource.CreateLinkedTokenSource(
                gameObject.GetCancellationTokenOnDestroy(),
                MainMenu.CancellationTokenOnReset).Token
            ).Forget();
            
            // PlayerPhysicsBody.Rigidbody.Rotat
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            PlayerMovement.Enabled = false;
            PlayerMovement.CanMove = false;
            Vector2 mousePos = MainCamera.WorldMousePos;

            bool cancelled = await RotateTowardsPositionTask(mousePos, cancellationToken)
                .SuppressCancellationThrow();
            
            if (!cancelled)
            {
                await ReleaseParticlesTask(mousePos, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();

                webQueue.Enqueue(PoolManager.GetEffect<PlayerWeb>(position: mousePos));
                RemoveExcessWeb(maxAmount);
            }
            
            PlayerMovement.Enabled = true;
            PlayerMovement.CanMove = true;
        }

        private async UniTask RotateTowardsPositionTask(Vector2 mousePos, CancellationToken cancellationToken)
        {
            Vector2 playerPos = PlayerPhysicsBody.Position;
            Vector2 rotatePos = playerPos + (PlayerPhysicsBody.Position - mousePos);
            
            float direction = PhysicsUtility.RotationTowards(
                playerPos, 
                PlayerPhysicsBody.Rotation, 
                rotatePos);

            float rotationDiff = float.MaxValue;
            bool cancelled = false;
            float fallbackTime = 0;
            do
            {
                rotationDiff = Mathf.Abs(PlayerPhysicsBody.Rotation - direction);
                PlayerPhysicsBody.Rigidbody.RotateTowardsPosition(
                    rotatePos,
                    Mathf.Min(20, rotationDiff));

                fallbackTime += Time.fixedDeltaTime;
                cancelled = await UniTask
                    .DelayFrame(1, PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();

            } while (!cancelled && fallbackTime < .5f && rotationDiff > 5f);
        }

        private async UniTask ReleaseParticlesTask(Vector2 mousePos, CancellationToken cancellationToken)
        {
            float distance = Vector2.Distance(PlayerPhysicsBody.Position, mousePos);
            float lifetime = distance * 0.15f;
            float coneAngleDeg = Mathf.Min(Mathf.Asin(1.6f / distance) * Mathf.Rad2Deg, 80);
            shapeModule.angle = coneAngleDeg;
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(lifetime, lifetime + 0.25f);

            particles.Play();
            // Wait for particles burst to be released
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}