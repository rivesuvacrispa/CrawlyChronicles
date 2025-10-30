using System;
using System.Collections;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.AI.Locators;
using SoundEffects;
using UnityEngine;
using UnityEngine.Rendering;
using Util;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour, ILocatorTarget
    {
        [SerializeField] private float knockbackResistance = 0.5f;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private float comboDashSpeedAmplifier;

        private readonly int idleHash = Animator.StringToHash("PlayerSpriteIdle");
        private readonly int walkHash = Animator.StringToHash("PlayerSpriteWalk");
        
        private float previousRotation;
        private CancellationTokenSource knockbackCts;
        
        private static Rigidbody2D rb;
        public static float MoveSpeedAmplifier { get; set; } = 1;
        public static Vector2 Position => rb.position;
        public static float Rotation => rb.rotation;

        public bool InAttackDash { get; private set; }



        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // Debug.Log(volume.profile.TryGet(out motionBlur));
        }

        private void FixedUpdate()
        {
            Vector2 mousePos = MainCamera.WorldMousePos;
            float currentRotation = rb.rotation;
            rb.RotateTowardsPosition(mousePos, PlayerManager.PlayerStats.RotationSpeed);

            if (Input.GetMouseButton(1))
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
            spriteAnimator.Play(walkHash);
        }

        private void PlayIdle()
        {
            PlayerAudioController.Instance.StopState();
            spriteAnimator.Play(idleHash);
        }
        
        public void Knockback(Vector2 attacker, float force)
        {
            rb.AddClampedForceBackwards(attacker, force * (1 - knockbackResistance), ForceMode2D.Impulse);
            CancelKnockback();
            knockbackCts = new CancellationTokenSource();
            KnockbackTask(knockbackCts.Token)
                .AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy())
                .Forget();
        }

        public async UniTask Dash(float duration, CancellationToken cancellationToken)
        {
            CancelKnockback();
            
            Vector2 position = MainCamera.WorldMousePos;
            float direction = PhysicsUtility.RotationTowards(rb.position, rb.rotation, position, 360);
            bool facingStraight = Mathf.Abs(rb.rotation - direction) < 30f;
            
            if (facingStraight)
                await StraightDashTask(position, duration, cancellationToken: cancellationToken);
            else
                await SideDashTask(position, direction, duration, cancellationToken: cancellationToken);

        }

        public async UniTask ComboDash(float duration, float speed, CancellationToken cancellationToken)
        {
            CancelKnockback();

            await ComboDashTask(duration, speed, cancellationToken: cancellationToken);
        }


        private async UniTask SideDashTask(Vector2 position, float direction, float duration, CancellationToken cancellationToken)
        {
            InAttackDash = true;
            enabled = false;
            spriteAnimator.Play(idleHash);
            rb.AddClampedForceTowards(position, PlayerManager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            float t = 0f;
            while (t < duration && Mathf.Abs(rb.rotation - direction) > 10f)
            {
                rb.RotateTowardsPosition(position, 10 / PlayerSizeManager.CurrentSize);
                t += Time.fixedDeltaTime;
                await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
            }

            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            enabled = true;
            InAttackDash = false;
        }

        private async UniTask StraightDashTask(Vector2 position, float duration, CancellationToken cancellationToken)
        {
            InAttackDash = true;
            enabled = false;
            spriteAnimator.Play(idleHash);
            rb.AddClampedForceTowards(position, PlayerManager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            enabled = true;
            InAttackDash = false;
        }

        private async UniTask ComboDashTask(float duration, float speed, CancellationToken cancellationToken)
        {
            InAttackDash = true;
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
                await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
            }
            
            enabled = true;
            InAttackDash = false;
            rb.angularVelocity = 0;
            rb.RotateTowardsPosition(MainCamera.WorldMousePos, 360);
        }

        private async UniTask KnockbackTask(CancellationToken cancellationToken)
        {
            enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: cancellationToken);
            enabled = true;
        }

        private void CancelKnockback()
        {
            knockbackCts?.Cancel();
            knockbackCts?.Dispose();
            knockbackCts = null;
        }

        public static void AddForce(Vector2 force) => rb.AddForce(force);
        public static void Teleport(Vector2 pos) => rb.position = pos;
    }
}