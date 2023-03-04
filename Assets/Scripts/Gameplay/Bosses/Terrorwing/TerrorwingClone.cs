using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class TerrorwingClone : MonoBehaviour
    {
        [SerializeField] private ParticleSystem radialParticals;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private TrailRenderer[] trails = new TrailRenderer[4];
        [SerializeField] private BodyPainter[] painters = new BodyPainter[5];
        [SerializeField] private TerrorwingProjectileSpawner[] projectileSpawners;
        [SerializeField] private bool original;

        [Header("Disable upon death")] 
        [SerializeField] private new Light2D light;
        [SerializeField] private ParticleSystem bodyParticles1;
        [SerializeField] private ParticleSystem bodyParticles2;
        
        private bool alive;


        public TerrorwingProjectileSpawner[] ProjectileSpawners => projectileSpawners;
        public void SetSimulated(bool isSimulated)
        {
            if (!isSimulated) rb.rotation = 0;
            rb.simulated = isSimulated;
        }

        private void Start()
        {
            if (original) alive = true;
        }

        public async UniTask SetActive(bool isActive, float fadeDuration, CancellationToken cancellationToken = default)
        {
            if(alive != isActive)
            {
                gameObject.SetActive(true);
                SetTrailsActive(false);
                
                if (isActive)
                {
                    if (original) fadeDuration *= 0.75f;
                    foreach (BodyPainter painter in painters) painter.FadeIn(fadeDuration);
                    alive = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);
                    SetTrailsActive(true);
                }
                else
                {
                    if (original) fadeDuration *= 1.5f;
                    foreach (BodyPainter painter in painters) painter.FadeOut(fadeDuration);
                    alive = false;
                    await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);
                    gameObject.SetActive(false);
                }
            }
        }

        public void ResetColor()
        {
            foreach (BodyPainter painter in painters) 
                painter.ResetColor(Color.white);
        }
        
        private void SetTrailsActive(bool isActive)
        {
            foreach (TrailRenderer trail in trails) trail.emitting = isActive;
        }

        public void ShootRadial()
        {
            radialParticals.Play();
        }

        public async UniTask ShootBullets(TerrorwingProjectile projectile, int amount, float delay, CancellationToken cancellationToken = default)
        {
            for(int i = 0; i < amount; i++)
            {
                projectileSpawners[UnityEngine.Random.Range(0, 4)].Spawn(projectile);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }
        }

        public void UpdatePosition(Vector2 pos, Vector2 playerPos)
        {
            transform.localPosition = pos;
            rb.RotateTowardsPosition(playerPos, 360);
        }

        public void DieIfClone()
        {
            if (original || !gameObject.activeInHierarchy) return;
            
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
        
        public async UniTask Die(CancellationToken cancellationToken = default)
        {
            bodyParticles1.Stop();
            bodyParticles2.Stop();
            SetTrailsActive(false);
            ResetColor();

            foreach (BodyPainter painter in painters)
            {
                painter.SetSortingLayer("Ground");
                painter.SetSortingOrder(100);
                painter.SetMaterial(GlobalDefinitions.DefaultSpriteMaterial);
                painter.Paint(GlobalDefinitions.DeathGradient, 1f);
                if(!painter.TryGetComponent(out Rigidbody2D partRb)) 
                    partRb = painter.gameObject.AddComponent<Rigidbody2D>();
                partRb.transform.localScale = Vector3.one;
                partRb.simulated = true;
                partRb.gravityScale = 0;
                partRb.drag = 1f;
                partRb.angularDrag = 2f;
                partRb.angularVelocity = 720f;
                partRb.AddForce(UnityEngine.Random.insideUnitCircle.normalized * 2f, 
                    ForceMode2D.Impulse);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(6f), cancellationToken: cancellationToken);
            await SetActive(false, 2f, cancellationToken: cancellationToken);
        }
        
        // Used by animator
        private void DisableAnimator()
        {
            GetComponent<Animator>().enabled = false;
            light.enabled = false;
        }
    }
}