using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class Terrorwing : MonoBehaviour
    {
        public TerrorwingPattern debug_PatternToSet;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem deathParticles;
        [Header("Refs")]
        [SerializeField] private TerrorwingProjectile projectilePrefab;
        [SerializeField] private TerrorwingClone original;
        [SerializeField] private TerrorwingClone[] allClones = new TerrorwingClone[4];
        [Header("Stats")]
        [SerializeField] private float flySpeed;
        [SerializeField] private float rotationSpeed;

        [Header("Swipe attack")] 
        [SerializeField] private int numberOfAttacks;
        [SerializeField] private float swipeAttackSpeed;
        [SerializeField] private float swipeAttackDistance;
        [SerializeField] private float distanceToComeback;
        [Header("Bombardier")] 
        [SerializeField] private float shootingSpeed;
        [SerializeField] private float aimScatter;
        [SerializeField] private float orbitTime;
        [SerializeField] private int orbitsAmount;
        [SerializeField] private float orbitSpeed;

        [Header("Bullet hell")] 
        [SerializeField] private TerrorwingBulletHell bulletHell;
        [SerializeField] private float bulletHellDuration;

        [Header("Illusions")]
        [SerializeField] private int illusionCycleAmount;
        [SerializeField] private float illusionRotationSpeed;
        [SerializeField] private float illusionsDistance;
        [SerializeField] private float illusionsFadeTime;

        private TerrorwingPattern currentPattern = TerrorwingPattern.Spawn;
        private CancellationTokenSource destructionCts;
        private CancellationTokenSource shootingCts;
        private CancellationTokenSource illusionsCts;
        private Rigidbody2D rb;

        private readonly TerrorwingPattern[] statePatterns = {
            TerrorwingPattern.Illusions,
            TerrorwingPattern.SwipeAttack,
            TerrorwingPattern.Bombardier,
            TerrorwingPattern.BulletHell
        };

        private readonly Stack<TerrorwingPattern> patternSequence = new(3);

        private bool destroyed;
        private bool died;



        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void BuildPatternSequence()
        {
            var patterns = statePatterns.OrderBy(_ => Random.value);
            patternSequence.Clear();
            foreach (TerrorwingPattern pattern in patterns) 
                patternSequence.Push(pattern);
        }
        
        private void Start()
        {
            Bossbar.Instance.SetMaxHealth(99999);
            Bossbar.Instance.SetName("Terrorwing");
            Bossbar.Instance.SetActive(true);
            ForcePattern(debug_PatternToSet);
        }

        public void ForcePattern(TerrorwingPattern pattern)
        {
            BuildPatternSequence();
            patternSequence.Pop();
            patternSequence.Push(pattern);
            
            DisposeAll();
            destructionCts = new CancellationTokenSource();
            var token = destructionCts.Token;
            
            PatternsTask(cancellationToken: token).Forget();
        }

        private void OnDestroy()
        {
            destroyed = true;
            DisposeAll();
        }

        private void DisposeAll()
        {
            DisposeTokenSource(destructionCts);
            destructionCts = null;
            DisposeTokenSource(shootingCts);
            shootingCts = null;
            DisposeTokenSource(illusionsCts);
            illusionsCts = null;
        }

        private void DisposeTokenSource(CancellationTokenSource cts)
        {
            if(cts is null) return;
            cts.Cancel();
            cts.Dispose();
        }
        
        private async UniTask SwipeAttackTask(CancellationToken cancellationToken)
        {
            int counter = numberOfAttacks;
            while (counter > 0 && currentPattern == TerrorwingPattern.SwipeAttack)
            {
                await SingleSwipeAttackTask(cancellationToken);
                counter--;
            }
        }

        private async UniTask SingleSwipeAttackTask(CancellationToken cancellationToken)
        {
            await CustomCoroutines.MoveUntilFacingAndCloseEnough(rb, Player.Manager.Instance.Transform, flySpeed, rotationSpeed, swipeAttackDistance,
                cancellationToken: cancellationToken);
            await ResetSpeed(cancellationToken);
                
            rb.AddClampedForceTowards(Player.Movement.Position, swipeAttackSpeed, ForceMode2D.Impulse);

            await KeepDistanceTask(distanceToComeback, cancellationToken: cancellationToken);
        }

        private async UniTask KeepDistanceTask(float distance, Transform target = default, CancellationToken cancellationToken = default)
        {
            if (target == default) target = Player.Manager.Instance.Transform;
            await UniTask.Delay(TimeSpan.FromMilliseconds(250f), cancellationToken: cancellationToken);
            await CustomCoroutines.WaitUntilDistanceGained(rb, target, flySpeed, rotationSpeed, + 2, cancellationToken: cancellationToken);
            await CustomCoroutines.WaitUntilDistanceReached(rb, target, flySpeed, rotationSpeed, distance, cancellationToken: cancellationToken);
        }

        private async UniTask ShootTask(CancellationToken cancellationToken)
        {
            while (currentPattern == TerrorwingPattern.Bombardier)
            {
                var spawner = original.ProjectileSpawners[Random.Range(0, 4)];
                var projectile = spawner.Spawn(projectilePrefab);
                var scatter = Random.insideUnitCircle * aimScatter;
                var target = Player.Movement.Position + (Vector2) Player.Manager.Instance.Transform.up + scatter;
                projectile.Target = target;
                await UniTask.Delay(TimeSpan.FromSeconds(shootingSpeed), cancellationToken: cancellationToken);
            }
        }

        private async UniTask OrbitWithAttackTask(CancellationToken cancellationToken)
        {
            await KeepDistanceTask(swipeAttackDistance, cancellationToken: cancellationToken);

            var axis = new Vector3(0, 0, 1);
            int orbitCounter = orbitsAmount;

            while (orbitCounter > 0 && currentPattern == TerrorwingPattern.Bombardier)
            {
                axis *= Random.value >= 0.5f ? 1 : -1;
                shootingCts = new CancellationTokenSource();
                ShootTask(shootingCts.Token).Forget();
                
                float t = orbitTime;
                while (t > 0)
                {
                    Vector2 playerPos = Player.Movement.Position;
                    transform.RotateAround(playerPos, axis, orbitSpeed * Time.deltaTime);
                    rb.RotateTowardsPosition(playerPos, 360);
                    t -= Time.deltaTime;
                    await UniTask.Yield(cancellationToken: cancellationToken);
                }
                animator.Play("TerrorwingFly");
                DisposeTokenSource(shootingCts);
                shootingCts = null;
                
                orbitCounter--;

                await SingleSwipeAttackTask(cancellationToken);
            }
        }

        private async UniTask BulletHellTask(CancellationToken cancellationToken)
        {
            await CustomCoroutines.WaitUntilDistanceReached(rb, GlobalDefinitions.MapCenter, flySpeed * 0.33f, rotationSpeed, 1, cancellationToken: cancellationToken);
            await UniTask.WhenAll(
                bulletHell.StartBulletHell(3, bulletHellDuration, cancellationToken: cancellationToken),
                BulletHellAnimationTask(cancellationToken));
        }

        private async UniTask IllusionsTask(CancellationToken cancellationToken)
        {
            illusionsCts = new CancellationTokenSource();
            int counter = illusionCycleAmount;
            await CustomCoroutines.WaitUntilDistanceReached(rb, Player.Manager.Instance.Transform, flySpeed, rotationSpeed, 3, cancellationToken: cancellationToken);
            await FadeOriginal(false, cancellationToken);

            IllusionsRotationTask(illusionsCts.Token).Forget();
            
            while (counter > 0 && currentPattern == TerrorwingPattern.Illusions)
            {
                allClones = allClones.OrderBy(_ => Random.value).ToArray();

                await FadeAll(true, cancellationToken);
                if(Random.value >= 0.5f) 
                    allClones[Random.Range(0, 4)].ShootBullets(projectilePrefab, 8, 0.25f, cancellationToken).Forget();
                else
                {
                    foreach (TerrorwingClone clone in allClones) 
                        clone.ShootRadial();
                }
                await FadeAll(false, cancellationToken);
                counter--;
            }

            DisposeTokenSource(illusionsCts);
            illusionsCts = null;
            SetClonesSimulated(false);
            original.transform.localPosition = Vector3.zero;
            original.transform.rotation = Quaternion.identity;
            await UniTask.WhenAll(
                FadeOriginal(true,  cancellationToken),
                KeepDistanceTask(5, cancellationToken: cancellationToken));
        }
        

        private async UniTask IllusionsRotationTask(CancellationToken cancellationToken)
        {
            SetClonesSimulated(true);
            float angle = 0;
            while (currentPattern == TerrorwingPattern.Illusions)
            {
                var playerPos = Player.Movement.Position;
                rb.position = playerPos;
                for (int i = 0; i < 4; i++)
                {
                    var clone = allClones[i];
                    float a = angle + i * Mathf.PI / 2;
                    Vector2 pos = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * illusionsDistance;
                    clone.UpdatePosition(pos, playerPos);
                }

                angle += illusionRotationSpeed * Time.deltaTime;
                await UniTask.Yield(cancellationToken);
            }
            SetClonesSimulated(false);
        }

        private async UniTask FadeOriginal(bool isActive, CancellationToken cancellationToken)
        {
            await original.SetActive(isActive, illusionsFadeTime, cancellationToken: cancellationToken);
        }
        
        private async UniTask FadeAll(bool isActive, CancellationToken cancellationToken)
        {
            IEnumerable<UniTask> tasks = new[]
            {
                allClones[0].SetActive(isActive, illusionsFadeTime, cancellationToken: cancellationToken),
                allClones[1].SetActive(isActive, illusionsFadeTime, cancellationToken: cancellationToken),
                allClones[2].SetActive(isActive, illusionsFadeTime, cancellationToken: cancellationToken),
                allClones[3].SetActive(isActive, illusionsFadeTime, cancellationToken: cancellationToken)
            };
            await UniTask.WhenAll(tasks);
        }

        private void SetClonesSimulated(bool isSimulated)
        {
            foreach (var clone in allClones) clone.SetSimulated(isSimulated);
        }

        private async UniTask BulletHellAnimationTask(CancellationToken cancellationToken)
        {
            float t = bulletHellDuration;
            while (t > 0)
            {
                rb.rotation += 360 * Time.deltaTime;
                t -= Time.deltaTime;
                await UniTask.Yield(cancellationToken);
            }
        }

        private async UniTask ResetSpeed(CancellationToken cancellationToken)
        {
            rb.velocity = Vector2.zero;
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: cancellationToken);
            await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);
        }


        private async UniTask DeathTask(CancellationToken cancellationToken)
        {
            await ResetSpeed(cancellationToken);
            rb.simulated = false;
            Bossbar.Instance.Die();
            animator.Play("TerrorwingDeath");
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancellationToken);
            deathParticles.Play();
            await original.Die(cancellationToken: cancellationToken);
            Destroy(gameObject);
        }
        
        private async UniTask PatternsTask(CancellationToken cancellationToken = default)
        {
            while (!died && !destroyed && patternSequence.TryPop(out var pattern))
            {
                bool canceled;
                currentPattern = pattern;
                Debug.Log($"Playing {pattern}");
                switch (pattern)
                {
                    case TerrorwingPattern.Illusions:
                        canceled = await IllusionsTask(cancellationToken).SuppressCancellationThrow();
                        if(canceled && !destroyed)
                        {
                            DisposeTokenSource(illusionsCts);
                            SetClonesSimulated(false);
                            foreach (TerrorwingClone clone in allClones) clone.DieIfClone();
                            original.ResetColor();
                        }
                        break;
                    case TerrorwingPattern.SwipeAttack:
                        await SwipeAttackTask(cancellationToken);
                        break;
                    case TerrorwingPattern.Bombardier:
                        await OrbitWithAttackTask(cancellationToken);
                        break;
                    case TerrorwingPattern.BulletHell:
                        canceled = await BulletHellTask(cancellationToken).SuppressCancellationThrow();
                        if(canceled && !destroyed) bulletHell.StopBulletHell();
                        break;
                    case TerrorwingPattern.Death:
                        died = true;
                        await DeathTask(cancellationToken);
                        break;
                }
                
                if(patternSequence.Count == 0) BuildPatternSequence();
            }
        }
    }
}