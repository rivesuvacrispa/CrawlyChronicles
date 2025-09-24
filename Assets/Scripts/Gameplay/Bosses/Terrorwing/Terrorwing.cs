using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using GameCycle;
using Gameplay.Map;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.Terrorwing
{
    public class Terrorwing : Boss, IDamageableEnemy, IEnemyAttack, IImpactable
    {
        public TerrorwingPattern debug_PatternToSet;
        public bool debug_DieFromPlayer;
        [Header("Refs")] 
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private TerrorwingHitbox mainHitbox;
        [SerializeField] private TerrorwingProjectile projectilePrefab;
        [SerializeField] private TerrorwingClone original;
        [SerializeField] private TerrorwingClone[] allClones = new TerrorwingClone[4];
        [Header("Stats")]
        [SerializeField] private float flySpeed;
        [SerializeField] private float rotationSpeed;
        [Header("Swipe attack")] 
        [SerializeField] private Vector2Int numberOfAttacks;
        [SerializeField] private float swipeAttackSpeed;
        [Header("Bombardier")] 
        [SerializeField] private float aimScatter;
        [SerializeField] private float orbitTime;
        [SerializeField] private Vector2Int orbitsAmount;
        [SerializeField] private float orbitSpeed;
        [Header("Bullet hell")] 
        [SerializeField] private TerrorwingBulletHell bulletHell;
        [SerializeField] private float bulletHellDuration;
        [Header("Illusions")]
        [SerializeField] private Vector2Int illusionCycleAmount;
        [SerializeField] private float illusionRotationSpeed;
        [SerializeField] private float illusionsDistance;

        private TerrorwingPattern currentPattern = TerrorwingPattern.SwipeAttack;
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

        private static readonly int DeathAnimHash = Animator.StringToHash("TerrorwingDeath");
        private static readonly int FlyAnimHash = Animator.StringToHash("TerrorwingFly");
        private static readonly int AttackAnimHash = Animator.StringToHash("TerrorwingAttack");
        
        private readonly Stack<TerrorwingPattern> patternSequence = new(3);

        private bool destroyed;
        private bool died;
        private float currentHealth;


        
        private void Awake() => rb = GetComponent<Rigidbody2D>();

        // Boss
        protected override void Enrage() => PlayerManager.Instance.Die(true);

        public override void Flee() => Die(false);

        protected override void Start()
        {
            currentHealth = TerrorwingDefinitions.MaxHealth;
            Bossbar.Instance.SetMaxHealth(currentHealth);
            base.Start();
            ForcePattern(debug_PatternToSet);
        }

        protected override void OnDestroy()
        {
            destroyed = true;
            DisposeAll();
            base.OnDestroy();
        }

        protected override bool InvokeDestructionEvent()
        {
            if (base.InvokeDestructionEvent())
            {
                mainHitbox.Die();
                attackGO.SetActive(false);
                return true;
            }
            return false;
        }


        private void BuildPatternSequence()
        {
            var patterns = statePatterns.OrderBy(_ => Random.value);
            patternSequence.Clear();
            foreach (TerrorwingPattern pattern in patterns) 
                patternSequence.Push(pattern);
        }

        public void ForcePattern(TerrorwingPattern pattern)
        {
            BuildPatternSequence();
            patternSequence.Pop();
            patternSequence.Push(pattern);
            
            DisposeAll();
            destructionCts = new CancellationTokenSource();
            PatternsTask(cancellationToken: destructionCts.Token).Forget();
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
            int counter = Random.Range(numberOfAttacks.x, numberOfAttacks.y + 1);
            while (counter > 0 && currentPattern == TerrorwingPattern.SwipeAttack)
            {
                await SingleSwipeAttackTask(cancellationToken);
                counter--;
            }
        }

        private async UniTask SingleSwipeAttackTask(CancellationToken cancellationToken)
        {
            await TaskUtility.MoveUntilFacingAndCloseEnough(rb, PlayerManager.Instance.Transform, flySpeed, 
                rotationSpeed, TerrorwingDefinitions.SwipeAttackDistance, cancellationToken: cancellationToken);
            await ResetSpeed(cancellationToken);
                
            mainHitbox.Disable();
            animator.Play(AttackAnimHash);
            attackGO.SetActive(true);
            rb.AddClampedForceTowards(PlayerMovement.Position, swipeAttackSpeed, ForceMode2D.Impulse);
            
            await KeepDistanceTask(TerrorwingDefinitions.SwipeAttackDistance, cancellationToken: cancellationToken);
            attackGO.SetActive(false);
            animator.Play(FlyAnimHash);
            mainHitbox.Enable();
        }

        private async UniTask KeepDistanceTask(float distance, Transform target = default, CancellationToken cancellationToken = default)
        {
            if (target == default) target = PlayerManager.Instance.Transform;
            await UniTask.Delay(TimeSpan.FromMilliseconds(250f), cancellationToken: cancellationToken);
            await TaskUtility.MoveUntilFacingAndCloseEnough(rb, null, flySpeed, rotationSpeed, 2.5f,
                staticTarget: target.position + (Vector3) Random.insideUnitCircle.normalized * 5, 
                cancellationToken: cancellationToken);
            await TaskUtility.WaitUntilDistanceReached(rb, target, flySpeed, rotationSpeed, distance, cancellationToken: cancellationToken);
        }

        private async UniTask ShootTask(CancellationToken cancellationToken)
        {
            while (currentPattern == TerrorwingPattern.Bombardier)
            {
                var spawner = original.ProjectileSpawners[Random.Range(0, 4)];
                var projectile = spawner.Spawn(projectilePrefab);
                var scatter = Random.insideUnitCircle * aimScatter;
                var target = PlayerMovement.Position + (Vector2) PlayerManager.Instance.Transform.up + scatter;
                projectile.Target = target;
                await UniTask.Delay(TimeSpan.FromSeconds(TerrorwingDefinitions.BombardierShootingSpeed),
                    cancellationToken: cancellationToken);
            }
        }

        private async UniTask OrbitWithAttackTask(CancellationToken cancellationToken)
        {
            var axis = new Vector3(0, 0, 1);
            int orbitCounter = Random.Range(orbitsAmount.x, orbitsAmount.y + 1);

            while (orbitCounter > 0 && currentPattern == TerrorwingPattern.Bombardier)
            {
                await KeepDistanceTask(TerrorwingDefinitions.SwipeAttackDistance,
                    cancellationToken: cancellationToken);
                
                axis *= Random.value > 0.5f ? 1 : -1;
                shootingCts = new CancellationTokenSource();
                ShootTask(shootingCts.Token).Forget();
                
                float t = orbitTime * Random.Range(0.8f, 1.2f);
                while (t > 0)
                {
                    Vector2 playerPos = PlayerMovement.Position;
                    transform.RotateAround(playerPos, axis, orbitSpeed * Time.deltaTime);
                    // rb.RotateTowardsPosition(playerPos, 360);
                    t -= Time.deltaTime;
                    await UniTask.Yield(cancellationToken: cancellationToken);
                }
                DisposeTokenSource(shootingCts);
                shootingCts = null;
                
                orbitCounter--;

                await SingleSwipeAttackTask(cancellationToken);
            }
        }

        private async UniTask BulletHellTask(CancellationToken cancellationToken)
        {
            await TaskUtility.WaitUntilDistanceReached(rb, MapManager.MapCenter, flySpeed * 0.33f, rotationSpeed, 1, cancellationToken: cancellationToken);
            await UniTask.WhenAll(
                bulletHell.StartBulletHell(3, bulletHellDuration, cancellationToken: cancellationToken),
                BulletHellAnimationTask(cancellationToken));
        }

        private async UniTask IllusionsTask(CancellationToken cancellationToken)
        {
            illusionsCts = new CancellationTokenSource();
            int counter = Random.Range(illusionCycleAmount.x, illusionCycleAmount.y + 1);
            await TaskUtility.WaitUntilDistanceReached(rb, PlayerManager.Instance.Transform, flySpeed, rotationSpeed, 3, cancellationToken: cancellationToken);
            await FadeOriginal(false, cancellationToken);

            rb.simulated = false;
            IllusionsRotationTask(illusionsCts.Token).Forget();
            
            while (counter > 0 && currentPattern == TerrorwingPattern.Illusions)
            {
                allClones = allClones.OrderBy(_ => Random.value).ToArray();

                await FadeAll(true, cancellationToken);
                if(Random.value > 0.5f) 
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
            rb.simulated = true;
            illusionsCts = null;
            transform.position = PlayerMovement.Position + Random.insideUnitCircle.normalized * 3;
            SetClonesSimulated(false);
            original.transform.localPosition = Vector3.zero;
            original.transform.localRotation = Quaternion.identity;
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
                Vector3 playerPos = PlayerMovement.Position;
                transform.position = playerPos;
                for (int i = 0; i < 4; i++)
                {
                    var clone = allClones[i];
                    float a = angle + i * Mathf.PI / 2;
                    Vector2 pos = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * illusionsDistance;
                    clone.UpdateIllusionPosition(pos, playerPos);
                }

                angle += illusionRotationSpeed * Time.deltaTime;
                await UniTask.Yield(cancellationToken);
            }

            SetClonesSimulated(false);
        }

        private async UniTask FadeOriginal(bool isActive, CancellationToken cancellationToken)
        {
            await original.SetActive(isActive, TerrorwingDefinitions.IllusionsFadeTime, cancellationToken: cancellationToken);
        }
        
        private async UniTask FadeAll(bool isActive, CancellationToken cancellationToken)
        {
            float illusionsFadeTime = TerrorwingDefinitions.IllusionsFadeTime;
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
            rb.linearVelocity = Vector2.zero;
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: cancellationToken);
            await UniTask.WaitForFixedUpdate(cancellationToken: cancellationToken);
        }

        public void Die(bool fromPlayer)
        {
            died = true;
            DisposeAll();
            destructionCts = new CancellationTokenSource();
            DeathTask(destructionCts.Token, fromPlayer).Forget();
        }
        
        private async UniTask DeathTask(CancellationToken cancellationToken, bool fromPlayer)
        {
            await ResetSpeed(cancellationToken);
            rb.simulated = false;
            animator.Play(DeathAnimHash);
            deathParticles.Play();

            if (fromPlayer)
                base.Die();
            else
                InvokeDestructionEvent();

            await UniTask.WhenAll(
                original.Die(cancellationToken: cancellationToken, fromPlayer),
                UniTask.WaitUntil(() => !deathParticles.isPlaying, cancellationToken: cancellationToken));
            
            Destroy(gameObject);
        }
        
        private async UniTask PatternsTask(CancellationToken cancellationToken = default)
        {
            while (!died && !destroyed && patternSequence.TryPop(out var pattern))
            {
                bool canceled = false;
                currentPattern = pattern;
                switch (pattern)
                {
                    case TerrorwingPattern.Illusions:
                        canceled = await IllusionsTask(cancellationToken).SuppressCancellationThrow();
                        break;
                    case TerrorwingPattern.SwipeAttack:
                        canceled = await SwipeAttackTask(cancellationToken).SuppressCancellationThrow();
                        break;
                    case TerrorwingPattern.Bombardier:
                        canceled = await OrbitWithAttackTask(cancellationToken).SuppressCancellationThrow();
                        break;
                    case TerrorwingPattern.BulletHell:
                        canceled = await BulletHellTask(cancellationToken).SuppressCancellationThrow();
                        break;
                }

                if (canceled && !destroyed) 
                    CleanupStates();

                if(!died && !destroyed && patternSequence.Count == 0) BuildPatternSequence();
            }
        }

        private void CleanupStates()
        {
            DisposeTokenSource(illusionsCts);
            DisposeTokenSource(shootingCts);
            SetClonesSimulated(false);
            original.ResetColor();
            mainHitbox.Enable();
            animator.Play(FlyAnimHash);
            bulletHell.StopBulletHell();
            attackGO.SetActive(false);
            illusionsCts = null;
            shootingCts = null;
            rb.simulated = true;
            original.transform.localPosition = Vector3.zero;
            original.transform.localRotation = Quaternion.identity;
            foreach (TerrorwingClone clone in allClones) clone.DieIfClone();
        }
        
        
        
        // IDamageableEnemy
        public Transform Transform => transform;
        public float HealthbarOffsetY => 0;
        public float HealthbarWidth => 0;

        public float Damage(
            float damage, 
            Vector3 position = default, 
            float knockback = 0f, 
            float stunDuration = 0f, 
            Color damageColor = default,
            bool ignoreArmor = false,
            AttackEffect effect = null)
        {
            if(rb.simulated && !mainHitbox.Immune)
            {
                mainHitbox.Hit();
                original.PaintDamage();
            }
            
            StatRecorder.damageDealt += damage;
            currentHealth -= damage;
            
            if (currentHealth <= 0)
                Die(true);
            else
                Bossbar.Instance.Damage(damage);

            effect?.Impact(this, damage);

            return damage;
        }
        
        
        
        // IEnemyAttack
        public Vector3 AttackPosition => transform.position + (Vector3) Random.insideUnitCircle.normalized;
        public float AttackDamage => TerrorwingDefinitions.ContactDamage;
        public float AttackPower => 10;
    }
}