using System;
using System.Collections;
using Gameplay.Player;
using UnityEngine;
using Util;

namespace Gameplay.Enemies
{
    public class SandFunnel : MonoBehaviour
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private float radius;
        [SerializeField] private float power;

        private Transform player;
        private float currentPower;
        private float finalPower;

        private Coroutine stepRoutine;
        
        private void Awake() => player ??= Player.PlayerManager.Instance.Transform;
        private void OnEnable()
        {
            stepRoutine ??= StartCoroutine(StepRoutine(power));
            particleSystem.Play();
        }

        private void OnDisable()
        {
            stepRoutine ??= StartCoroutine(StepRoutine(0));
            particleSystem.Stop();
        }

        private void FixedUpdate()
        {
            Vector2 playerPos = player.position;
            Vector2 pos = transform.position;
            float dist = (playerPos - pos).sqrMagnitude;
            if(dist > radius) return;

            PlayerPhysicsBody.Rigidbody.AddForce(PhysicsUtility.GetVelocityBackwards(
                pos, playerPos, currentPower * (1 - dist / radius)));
        }

        private IEnumerator StepRoutine(float final)
        {
            finalPower = final;
            while (Math.Abs(currentPower - finalPower) > float.Epsilon)
            {
                currentPower = Mathf.MoveTowards(currentPower, finalPower, Time.fixedDeltaTime * 1.25f);
                yield return new WaitForFixedUpdate();
            }
            currentPower = finalPower;
        }
    }
}