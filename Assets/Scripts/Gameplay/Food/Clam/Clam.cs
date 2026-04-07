using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Gameplay.Enemies;
using Gameplay.Food.Foodbeds;
using Gameplay.Player;
using Hitboxes;
using SoundEffects;
using UnityEngine;
using Util;
using Util.Components;
using Util.Interfaces;
using Random = UnityEngine.Random;

namespace Gameplay.Food.Clam
{
    public class Clam : Foodbed, IDamageable
    {
        [SerializeField] private DamageableHitbox hitbox;
        [SerializeField] private BodyPainter bodyPainter;
        
        private bool closed;
        public override bool CanGrow => false;
        private CancellationTokenSource cancellationTokenSource;
        
        

        protected override void Awake()
        {
            base.Awake();
            UpdateSprite();
        }

        protected override void Start()
        {
            base.Start();
            SetHealth(MaxHealth);
        }

        private void SetClosed(bool state)
        {
            if (closed == state) return;
            closed = state;
            UpdateSprite();
        }

        protected override Sprite GetGrowthSprite(int amount)
        {
            if (CurrentHealth < MaxHealth)
            {
                float damagePercent = Mathf.Clamp01(CurrentHealth / MaxHealth);
                float damageIndex = Mathf.Lerp(5, 2, damagePercent);
                print($"Damage index: {damageIndex}");
                return scriptable.GetGrowthSprite(Mathf.FloorToInt(damageIndex));
            };
            
            return scriptable.GetGrowthSprite(closed ? 0 : 1);
        }

        private void SetHealth(float amount)
        {
            CurrentHealth = amount;
            UpdateSprite();
        }

        public override bool CanInteract() => false;

        protected override void OnEatenByPlayer()
        {
        }

        public void OnTouch(Collision2D col)
        {
            if (closed) return;
            
            bool caught = false;
            bool caughtPlayer = false;
            if (col.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Die();
                caught = true;
            }
            else if (col.gameObject.TryGetComponent(out PlayerManager player))
            {
                player.Die(false);
                caught = true;
                caughtPlayer = true;
            }

            if (!caught) return;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            float openingDuration = caughtPlayer ? 30f : 10f;
            SetClosed(true);
            OpeningTask(openingDuration, gameObject.CreateCommonCancellationToken(cancellationTokenSource.Token)).Forget();
        }

        private async UniTask OpeningTask(float duration, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);

            SetClosed(false);
        }

        private async UniTask DestroyTask(CancellationToken cancellationToken)
        {
            bodyPainter.FadeOut(0.5f);
            await DOTween.Sequence()
                .Insert(0, bodyPainter.transform.DOScale(1.5f, 0.51f))
                .Insert(0, bodyPainter.transform.DOShakeRotation(0.51f, 20f, 30, 120f))
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);

            Destroy(gameObject);
        }

        private void Shake()
        {
            bodyPainter.transform.DOShakeRotation(0.5f, 20f, 30, 120f);
        }

        
        // INotificationProvider
        protected override bool CreateNotification => false;


        // IDamageable
        public float HealthbarOffsetY => 9999;
        public float HealthbarWidth => 100;
        public event IDamageable.DeathEvent OnDeath;
        public event IDamageable.DamageEvent OnDamageTaken;
        public float Armor => 0;
        public float CurrentHealth { get; set; }
        public float MaxHealth => 10f;
        public IDamageableHitbox Hitbox => hitbox;

        public void OnBeforeHit(DamageInstance damageInstance)
        {
            PlayerParryParticles.Play(damageInstance.position);
            PlayerAudioController.Instance.PlayReckoning();
            Shake();
            OnDamageTaken?.Invoke(this, damageInstance);
        }

        public void OnLethalHit(DamageInstance damageInstance)
        {
            OnDeath?.Invoke(this);
            ClamMeat meat = Instantiate(GlobalDefinitions.ClamMeatPrefab);
            meat.transform.position = transform.position;
            var rb = meat.GetComponent<Rigidbody2D>();
            rb.AddForce(Random.insideUnitCircle.normalized);
            rb.AddTorque(Random.value * 30f + 30f);
            DestroyTask(gameObject.CreateCommonCancellationToken()).Forget();
        }

        public void OnHit(DamageInstance damageInstance)
        {
            SetClosed(true);
            bodyPainter.Paint(new Gradient().FastGradient(Color.red, Color.white), GlobalDefinitions.EnemyImmunityDuration);
            UpdateSprite();
        }
    }
}