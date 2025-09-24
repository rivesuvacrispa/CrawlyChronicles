﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using GameCycle;
using Gameplay.Enemies;
using Gameplay.Map;
using Gameplay.Mutations.AttackEffects;
using Gameplay.Mutations.EntityEffects;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Bosses.AntColony
{
    public class AntQueen : Boss, IDamageableEnemy, IEnemyAttack, IImpactEffectAffectable
    {
        [Header("Refs")] 
        [SerializeField] private EffectController effectController;
        [SerializeField] private DamageableEnemyHitbox hitbox;
        [SerializeField] private AntQueenWallTrigger wallTrigger;
        [SerializeField] private List<Enemy> antsToHatch = new();
        [SerializeField] private AntColonyEgg eggPrefab;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private List<BodyPainter> painters = new();
        [SerializeField] private Transform eggsTransform;
        [Header("Stats")] 
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackDistance;
        [SerializeField] private int eggLayAmountScatter;
        [SerializeField] private int attacksAmount;
        [SerializeField] private int attacksAmountScatter;

        private CancellationTokenSource cts;
        private CancellationTokenSource eggLayingCts;
        private float currentHealth;
        private float maxHealth;
        private float currentMovespeed;
        private float currentDamage;

        public bool TouchingWalls { get; set; }
        
        private static readonly int WalkAnimHash = Animator.StringToHash("AntQueenWalk");
        private static readonly int IdleAnimHash = Animator.StringToHash("AntQueenIdle");
        private static readonly int EggsAnimHash = Animator.StringToHash("AntQueenLayingEggs");

        protected override void Start()
        {
            maxHealth = AntColonyDefinitions.Health;
            Bossbar.Instance.SetMaxHealth(maxHealth);
            base.Start();
            
            cts = new CancellationTokenSource();
            currentMovespeed = AntColonyDefinitions.MoveSpeed;
            currentDamage = AntColonyDefinitions.Damage;
            currentHealth = maxHealth;
            AITask(cts.Token).Forget();
        }

        private async UniTask AITask(CancellationToken cancellationToken)
        {
            while (enabled)
            {
                await AttackTask(cancellationToken);
                await EggLayingTask(cancellationToken);
            }
        }

        private async UniTask AttackTask(CancellationToken cancellationToken, bool infinite = false)
        {
            animator.Play(WalkAnimHash);
            int attackCounter = attacksAmount + 
                attacksAmountScatter - Random.Range(0, attacksAmountScatter + 1);

            while (attackCounter > 0 || infinite)
            {
                await TaskUtility.MoveUntilFacingAndCloseEnough(rb, PlayerManager.Instance.Transform,
                    currentMovespeed, currentMovespeed * 3f, attackDistance, cancellationToken);
                animator.speed = 3;
                attackGO.SetActive(true);
                rb.AddClampedForceTowards(Player.PlayerMovement.Position, attackSpeed, ForceMode2D.Impulse);
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                attackGO.SetActive(false);
                animator.speed = 1;
                attackCounter--;
            }
        }

        private async UniTask EggLayingTask(CancellationToken cancellationToken)
        {
            wallTrigger.SetEnabled(true);
            int eggCounter = AntColonyDefinitions.EggsAmount 
                + eggLayAmountScatter - Random.Range(0, eggLayAmountScatter + 1);

            await UniTask.WaitForFixedUpdate();
            while (eggCounter > 0)
            {
                if (TouchingWalls)
                {
                    DisposeTokenSource(eggLayingCts);
                    eggLayingCts = new CancellationTokenSource();
                    TaskUtility.StepTowardsWhileReachingDistance(rb, MapManager.MapCenter,
                        currentMovespeed, currentMovespeed * 3f, 1, eggLayingCts.Token).Forget();
                    await UniTask.WaitUntil(() => !TouchingWalls, cancellationToken: cancellationToken);
                    DisposeTokenSource(eggLayingCts);
                    eggLayingCts = null;
                }

                animator.Play(EggsAnimHash);
                await UniTask.Delay(TimeSpan.FromSeconds(1 / 3f), cancellationToken: cancellationToken);
                LayEgg();
                eggCounter--;
            }

            wallTrigger.SetEnabled(false);
        }

        private void LayEgg()
        {
            var egg = Instantiate(eggPrefab, MapManager.GameObjectsTransform);
            var position = eggsTransform.position;
            egg.transform.localPosition = position;
            egg.Rb.AddClampedForceBackwards(transform.position, 
                Random.Range(0.5f, 0.75f), ForceMode2D.Impulse);
            egg.Rb.angularVelocity = Random.Range(500f, 720f);
            egg.ToHatch = antsToHatch[Random.Range(0, antsToHatch.Count)];
        }

        protected override void Die()
        {
            base.Die();
            effectController.ClearAll();
            hitbox.Die();
            DisposeTokenSource(cts);
            DisposeTokenSource(eggLayingCts);
            eggLayingCts = null;
            cts = new CancellationTokenSource();
            DieTask(cts.Token).Forget();
        }
        
        private async UniTask DieTask(CancellationToken cancellationToken)
        {
            animator.Play(IdleAnimHash);
            rb.simulated = false;
            wallTrigger.SetEnabled(false);
            foreach (BodyPainter painter in painters) 
                painter.PlayDead(100);
            await UniTask.Delay(TimeSpan.FromSeconds(6f), cancellationToken: cancellationToken);
            foreach (BodyPainter painter in painters) 
                painter.FadeOut(2f);
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cancellationToken);
            Destroy(gameObject);
        }
        
        private void DisposeTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if(cancellationTokenSource is null) return;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        private async UniTask FleeTask(CancellationToken cancellationToken)
        {
            await TaskUtility.MoveUntilFacingAndCloseEnough(rb, null, currentMovespeed, currentMovespeed * 3f,
                1, cancellationToken, staticTarget: MapManager.GetRandomPointAroundMap(20));
            Destroy(gameObject);
        }
        
        
        
        // Overrides
        public override void Flee()
        {
            hitbox.Disable();
            currentMovespeed *= 1.5f;
            DisposeTokenSource(cts);
            DisposeTokenSource(eggLayingCts);
            cts = new CancellationTokenSource();
            FleeTask(cts.Token).Forget();
            base.Flee();
        }

        protected override void Enrage()
        {
            currentMovespeed *= 2f;
            currentDamage *= 2f;
            DisposeTokenSource(cts);
            DisposeTokenSource(eggLayingCts);
            eggLayingCts = null;
            cts = new CancellationTokenSource();
            AttackTask(cts.Token, true).Forget();
        }

        protected override void OnDestroy()
        {
            DisposeTokenSource(cts);
            cts = null;
            DisposeTokenSource(eggLayingCts);
            eggLayingCts = null;
            base.OnDestroy();
        }

        
        
        // IDamageableEnemy
        public Transform Transform => transform;
        public float HealthbarOffsetY => 0;
        public float HealthbarWidth => 0;

        public float Damage(float damage, Vector3 position, float knockback, float stunDuration, Color damageColor,
            bool ignoreArmor = false, AttackEffect effect = null)
        {
            if (hitbox.Immune) return 0;
            
            damage = ignoreArmor ? damage : PhysicsUtility.CalculateDamage(damage, AntColonyDefinitions.Armor);
            currentHealth -= damage;
            ((IDamageable)this).InvokeDamageTakenEvent(damage);
            
            if (currentHealth <= 0)
                Die();
            else
            {
                hitbox.Hit();
                Bossbar.Instance.Damage(damage);
                var gradient = new Gradient().FastGradient(damageColor, painters[0].Color);
                foreach (BodyPainter painter in painters) 
                    painter.Paint(gradient, GlobalDefinitions.EnemyImmunityDuration);
            }
            
            effect?.Impact(this, damage);
            
            return damage;
        }
        
        // IEnemyAttack
        public Vector3 AttackPosition => attackGO.transform.position;
        public float AttackDamage => currentDamage;
        public float AttackPower => 5;
        
        // IImpactEffectAffectable
        public void AddEffect<T>(EntityEffectData data) where T : EntityEffect
        {
            if(!hitbox.Dead) effectController.AddEffect<T>(data);
        }
    }
}