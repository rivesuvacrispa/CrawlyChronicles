using System;
using System.Collections;
using Camera;
using Gameplay.AI.Locators;
using Scripts.SoundEffects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Util;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour, ILocatorTarget
    {
        [SerializeField] private float knockbackResistance = 0.5f;
        [SerializeField] private Animator spriteAnimator;
        [SerializeField] private float comboDashSpeedAmplifier;
        [SerializeField] private Volume volume;

        private readonly int idleHash = Animator.StringToHash("PlayerSpriteIdle");
        private readonly int walkHash = Animator.StringToHash("PlayerSpriteWalk");

        private Coroutine dashRoutine;
        private float previousRotation;
        
        private static Rigidbody2D rb;
        public static float MoveSpeedAmplifier { get; set; } = 1;
        public static Vector2 Position => rb.position;
        public static float Rotation => rb.rotation;


        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // Debug.Log(volume.profile.TryGet(out motionBlur));
        }

        private void FixedUpdate()
        {
            Vector2 mousePos = MainCamera.WorldMousePos;
            float currentRotation = rb.rotation;
            rb.RotateTowardsPosition(mousePos, Manager.PlayerStats.RotationSpeed);

            if (Input.GetMouseButton(1))
            {
                PlayCrawl();
                rb.AddForce(transform.up * Manager.PlayerStats.MovementSpeed * MoveSpeedAmplifier);
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
            StartCoroutine(KnockbackRoutine());
        }

        public bool Dash(float duration, Action onEnd)
        {
            if (dashRoutine is null)
            {
                Vector2 position = MainCamera.WorldMousePos;
                float direction = PhysicsUtility.RotationTowards(rb.position, rb.rotation, position, 360);
                dashRoutine = StartCoroutine(Mathf.Abs(rb.rotation - direction) < 30f ?
                    StraightDashRoutine(position, duration, onEnd) : 
                    SideDashRoutine(position, direction, duration, onEnd));

                StopCoroutine(nameof(KnockbackRoutine));
                return true;
            }

            return false;
        }

        public bool ComboDash(float duration, float speed, Action onEnd)
        {
            if (dashRoutine is null)
            {
                dashRoutine = StartCoroutine(ComboDashRoutine(duration, speed, onEnd));
                return true;
            }

            return false;
        }


        private IEnumerator SideDashRoutine(Vector2 position, float direction, float duration, Action onEnd)
        {
            enabled = false;
            spriteAnimator.Play(idleHash);
            rb.AddClampedForceTowards(position, Manager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            float t = 0f;
            while (t < duration && Mathf.Abs(rb.rotation - direction) > 10f)
            {
                rb.RotateTowardsPosition(position, 10);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForEndOfFrame();
            enabled = true;
            dashRoutine = null;
            onEnd();
        }

        private IEnumerator StraightDashRoutine(Vector2 position, float duration, Action onEnd)
        {
            enabled = false;
            spriteAnimator.Play(idleHash);
            rb.AddClampedForceTowards(position, Manager.PlayerStats.AttackPower * MoveSpeedAmplifier, ForceMode2D.Impulse);

            yield return new WaitForSeconds(duration);
            yield return new WaitForEndOfFrame();
            enabled = true;
            dashRoutine = null;
            onEnd();
        }

        private IEnumerator ComboDashRoutine(float duration, float speed, Action onEnd)
        {
            enabled = false;
            rb.angularVelocity = speed;
            float t = 0;

            while (t < duration)
            {
                rb.rotation += speed;
                rb.AddClampedForceTowards(
                    MainCamera.WorldMousePos,
                    Manager.PlayerStats.AttackPower * MoveSpeedAmplifier * comboDashSpeedAmplifier,
                    ForceMode2D.Force,
                    maxAmplifier: comboDashSpeedAmplifier);

                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            enabled = true;
            dashRoutine = null;
            rb.angularVelocity = 0;
            rb.RotateTowardsPosition(MainCamera.WorldMousePos, 360);
            onEnd();
        }

        private IEnumerator KnockbackRoutine()
        {
            enabled = false;
            yield return new WaitForSeconds(0.25f);
            enabled = true;
        }

        public static void AddForce(Vector2 force) => rb.AddForce(force);
        public static void Teleport(Vector2 pos) => rb.position = pos;
    }
}