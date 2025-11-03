using System;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using SoundEffects;
using UnityEngine;
using Util;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private static PlayerMovement instance;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float knockbackResistance = 0.5f;
        [SerializeField] private float comboDashSpeedAmplifier;


        
        private float previousRotation;
        private static CancellationTokenSource knockbackCts;
        
        public static float MoveSpeedAmplifier { get; set; } = 1;


        public static bool CanMove { get; set; } = true;
        public static bool Enabled
        {
            get => instance.enabled;
            set => instance.enabled = value;
        }


        private PlayerMovement() => instance = this;
   

        private void FixedUpdate()
        {
            Vector2 mousePos = MainCamera.WorldMousePos;
            float currentRotation = rb.rotation;
            rb.RotateTowardsPosition(mousePos, PlayerManager.PlayerStats.RotationSpeed);

            if (CanMove && Input.GetMouseButton(1))
            {
                PlayCrawl();
                rb.AddForce(transform.up * (PlayerManager.PlayerStats.MovementSpeed * MoveSpeedAmplifier));
            }
            else if (Mathf.Abs(previousRotation - currentRotation) > 1f)
                PlayCrawl();
            else
                PlayIdle();
            
            previousRotation = currentRotation;
        }

        private void PlayCrawl()
        {
            PlayerAudioController.Instance.PlayCrawl();
            PlayerAnimator.PlayWalk();
        }

        private void PlayIdle()
        {
            PlayerAudioController.Instance.StopState();
            PlayerAnimator.PlayIdle();
        }
        
        public void Knockback(Vector2 attacker, float force)
        {
            rb.AddClampedForceBackwards(attacker, force * (1 - knockbackResistance), ForceMode2D.Impulse);
            CancelKnockback();
            knockbackCts = new CancellationTokenSource();
            KnockbackTask(
                CancellationTokenSource.CreateLinkedTokenSource(
                        knockbackCts.Token, 
                        gameObject.GetCancellationTokenOnDestroy())
                    .Token)
                .Forget();
        }

        public static async UniTask Dash(float duration, CancellationToken cancellationToken)
        {
            CancelKnockback();
            
            Vector2 position = MainCamera.WorldMousePos;
            float direction = PhysicsUtility.RotationTowards(PlayerPhysicsBody.Rigidbody.position, PlayerPhysicsBody.Rigidbody.rotation, position, 360);
            bool facingStraight = Mathf.Abs(PlayerPhysicsBody.Rigidbody.rotation - direction) < 30f;
            
            if (facingStraight)
                await instance.StraightDashTask(position, duration, cancellationToken: cancellationToken);
            else
                await instance.SideDashTask(position, direction, duration, cancellationToken: cancellationToken);

        }

        public static async UniTask ComboDash(float duration, float speed, CancellationToken cancellationToken)
        {
            CancelKnockback();

            await instance.ComboDashTask(duration, speed, cancellationToken: cancellationToken);
        }


        private async UniTask SideDashTask(Vector2 position, float direction, float duration, CancellationToken cancellationToken)
        {
            enabled = false;
            PlayerAnimator.PlayIdle();
            rb.AddClampedForceTowards(position, PlayerManager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            float t = 0f;
            while (t < duration && Mathf.Abs(rb.rotation - direction) > 10f)
            {
                rb.RotateTowardsPosition(position, 10 / PlayerSizeManager.CurrentSize);
                t += Time.fixedDeltaTime;
                bool cancelled = await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                
                if (cancelled) break;
            }

            await UniTask.DelayFrame(1, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            enabled = true;
        }

        private async UniTask StraightDashTask(Vector2 position, float duration, CancellationToken cancellationToken)
        {
            enabled = false;
            PlayerAnimator.PlayIdle();
            rb.AddClampedForceTowards(position, PlayerManager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken)
                .SuppressCancellationThrow()
                ;
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            enabled = true;
        }

        private async UniTask ComboDashTask(float duration, float speed, CancellationToken cancellationToken)
        {
            enabled = false;
            rb.angularVelocity = speed;
            float t = 0;

            while (t < duration)
            {
                rb.rotation += speed / PlayerSizeManager.CurrentSize;
                rb.AddClampedForceTowards(
                    MainCamera.WorldMousePos,
                    PlayerManager.PlayerStats.AttackPower * MoveSpeedAmplifier * comboDashSpeedAmplifier,
                    ForceMode2D.Force,
                    maxAmplifier: comboDashSpeedAmplifier);

                t += Time.fixedDeltaTime;
                bool cancelled = await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
                
                if (cancelled) break;
            }
            
            enabled = true;
            rb.angularVelocity = 0;
            rb.RotateTowardsPosition(MainCamera.WorldMousePos, 360);
        }

        private async UniTask KnockbackTask(CancellationToken cancellationToken)
        {
            enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            enabled = true;
        }

        public static void CancelKnockback()
        {
            knockbackCts?.Cancel();
            knockbackCts?.Dispose();
            knockbackCts = null;
        }

        public static void Teleport(Vector2 pos) => PlayerPhysicsBody.Rigidbody.position = pos;
    }
}