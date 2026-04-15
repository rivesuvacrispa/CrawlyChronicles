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
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class SpinneretGlands : ActiveAbility
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem debuffParticles;

        [SerializeField, MinMaxRange(1, 20)] private LevelInt websAmount = new LevelInt(4, 16);
        [SerializeField, MinMaxRange(0f, 1f)] private LevelFloat movementSlow = new LevelFloat(0.25f, 0.75f);
        [SerializeField, MinMaxRange(0f, 1f)] private LevelFloat rotationSlow = new LevelFloat(0.2f, 0.8f);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat movementSpeed = new LevelFloat(0.1f, 1f);
        [SerializeField, MinMaxRange(0f, 10f)] private LevelFloat rotationSpeed = new LevelFloat(0.5f, 5f);

        private ParticleSystem.MainModule mainModule;
        private ParticleSystem.ShapeModule shapeModule;
        private readonly Queue<PlayerWeb> webQueue = new();

        private int currentMaxAmount;
        private float currentMovementSlow;
        private float currentRotationSlow;
        private float currentMovementSpeed;
        private float currentRotationSpeed;
        public static SlowEffectData DebuffEffectData { get; private set; }
        public static StatEffectData BuffEffectData { get; private set; }
        public static ParticleSystem DebuffParticles { get; private set; }


        
        protected override void Awake()
        {
            base.Awake();
            DebuffParticles = debuffParticles;
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
            currentMaxAmount = websAmount.AtLvl(lvl);
            currentMovementSlow = movementSlow.AtLvl(lvl);
            currentRotationSlow = rotationSlow.AtLvl(lvl);
            currentMovementSpeed = movementSpeed.AtLvl(lvl);
            currentRotationSpeed = rotationSpeed.AtLvl(lvl);

            DebuffEffectData = new SlowEffectData(1, currentMovementSlow, currentRotationSlow);
            BuffEffectData = new StatEffectData(1, new PlayerStats(
                movementSpeed: currentMovementSpeed, 
                rotationSpeed: currentRotationSpeed)
            );
            
            if (Application.IsPlaying(this) && isActiveAndEnabled)
            {
                RemoveExcessWeb(currentMaxAmount);
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
            AttackController.Instance.CancelAttack();
            PlayerMovement.CancelKnockback();
                
            ActivateTask(CreateCommonCancellationToken()).Forget();
            
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
                RemoveExcessWeb(currentMaxAmount);
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

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new ILevelField[]
            {
                Scriptable.Cooldown,
                websAmount.UseKey(LevelFieldKeys.MAX_WEBS),
                movementSlow.UseKey(LevelFieldKeys.MOVEMENT_SLOW).UseFormatter(StatFormatter.PERCENT),
                rotationSlow.UseKey(LevelFieldKeys.ROTATION_SLOW).UseFormatter(StatFormatter.PERCENT),
                movementSpeed.UseKey(LevelFieldKeys.MOVEMENT_SPEED),
                movementSpeed.UseKey(LevelFieldKeys.ROTATION_SPEED),
            };
        }
    }
}