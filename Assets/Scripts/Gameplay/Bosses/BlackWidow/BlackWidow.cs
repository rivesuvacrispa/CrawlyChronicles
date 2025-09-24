using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player;
using UnityEditor;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BlackWidowWeb web;
        [SerializeField] private BlackWidowAnimatorHanging animatorHanging;
        [SerializeField] private BlackWidowAnimatorBody animatorBody;
        [SerializeField] private BlackWidowStormAttack stormAttack;
        [SerializeField] private BlackWidowCageAttack cageAttack;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private ParticleSystem acidBurstParticles;
        [Header("Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackDistance;
        [SerializeField] private float acidBurstMinRange = 3f;

        private BlackWidowWeb instantiatedWeb;

        

        private void Awake()
        {
            SwitchBodyHanging();
        }

        private void Start()
        {
            SpawnTask().Forget();
        }

        private void OnDestroy()
        {
            if (instantiatedWeb is not null) instantiatedWeb.Die().Forget();
        }

        /*
        private async UniTask SpawnTask()
        {
            instantiatedWeb = Instantiate(web);

            
            await animatorHanging.HangAtHeightAsync(3f);
            await UniTask.Delay(TimeSpan.FromSeconds(2f));

            cageAttack.enabled = true;
        }
        */

        
        private async UniTask SpawnTask()
        {
            instantiatedWeb = Instantiate(web);

            
            await animatorHanging.PlayToBottom();
            
            SwitchBodyTopDown();
            AITask(gameObject.GetCancellationTokenOnDestroy()).Forget();

            // cageAttack.enabled = true;
        }
        
        private async UniTask AITask(CancellationToken cancellationToken)
        {
            while (enabled)
            {
                await AttackTask(cancellationToken, true);
            }
        }

        private async UniTask AttackTask(CancellationToken cancellationToken, bool infinite = false)
        {
            int attackCounter = 3;

            while (attackCounter > 0 || infinite)
            {
                if ((transform.position - PlayerManager.Instance.transform.position).magnitude >= acidBurstMinRange)
                    acidBurstParticles.Play();
                
                await TaskUtility.MoveUntilFacingAndCloseEnough(rb, PlayerManager.Instance.Transform,
                    moveSpeed, rotationSpeed * 3, attackDistance, cancellationToken);
                
                animatorBody.SetSpeed(3);
                attackGO.SetActive(true);
                rb.AddClampedForceTowards(Player.PlayerMovement.Position, attackSpeed, ForceMode2D.Impulse);
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                attackGO.SetActive(false);
                animatorBody.SetSpeed(1);
                attackCounter--;

                
            }
        }


        private void SwitchBodyHanging()
        {
            animatorHanging.gameObject.SetActive(true);
            animatorBody.gameObject.SetActive(false);
        }

        private void SwitchBodyTopDown()
        {
            animatorHanging.gameObject.SetActive(false);
            animatorBody.gameObject.SetActive(true);
        }
    }
}