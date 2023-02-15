using System;
using System.Collections;
using Camera;
using Gameplay.AI.Locators;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour, ILocatorTarget
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float dashRotationSpeed;
        
        private static Rigidbody2D rb;
        private Coroutine dashRoutine;

        public static Vector2 Position => rb.position;
        public static Transform Transform => rb.transform;
        
        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            Vector2 mousePos = MainCamera.WorldMousePos;
            rb.rotation = RotateTowardsPosition(mousePos, rotationSpeed);

            if (Input.GetMouseButton(0))
            {
                rb.velocity = transform.up * moveSpeed;
            }
        }

        private float RotateTowardsPosition(Vector2 pos, float delta)
        {
            Vector2 rotateDirection = pos - rb.position;
            float angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) - Mathf.PI * 0.5f;
            return RotateTowardsAngle(angle, delta);
        }

        private float RotateTowardsAngle(float angle, float delta)
        {
            return Quaternion.RotateTowards(
                Quaternion.Euler(0, 0, rb.rotation), 
                quaternion.Euler(0, 0, angle),
                delta).eulerAngles.z;
        }
        
        public bool Dash(Vector2 position, float duration, float speed, Action onEnd)
        {
            if (dashRoutine is null)
            {
                float direction = RotateTowardsPosition(position, 360);
                if(Mathf.Abs(rb.rotation - direction) < 30f) 
                    dashRoutine = StartCoroutine(StraightDashRoutine(position, duration, speed * 0.75f, onEnd));
                else
                    dashRoutine = StartCoroutine(SideDashRoutine(position, direction, duration, speed, onEnd));
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

        
        private IEnumerator SideDashRoutine(Vector2 position, float direction, float duration, float speed, Action onEnd)
        {
            rb.velocity = (position - rb.position).normalized * speed;
            float t = 0f;
            enabled = false;

            while (t < duration && Mathf.Abs(rb.rotation - direction) > 10f)
            {
                rb.rotation = RotateTowardsPosition(position, dashRotationSpeed);
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            enabled = true;
            dashRoutine = null;
            onEnd();
        }
        
        private IEnumerator StraightDashRoutine(Vector2 position, float duration, float speed, Action onEnd)
        {
            rb.velocity = (position - rb.position).normalized * speed;
            float t = 0f;
            enabled = false;

            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

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
                rb.velocity = ((Vector2) MainCamera.WorldMousePos - rb.position).normalized * moveSpeed * 1.5f;
                t += Time.deltaTime;
                yield return null;
            }
            
            enabled = true;
            dashRoutine = null;
            rb.angularVelocity = 0;
            rb.rotation = RotateTowardsPosition(MainCamera.WorldMousePos, 360);
            onEnd();
        }

        public string LocatorTargetName => "Player";
    }
}