using Definitions;
using Timeline;
using UnityEngine;
using Util.Enums;
using Util.Interfaces;

namespace Gameplay.Food.Foodbeds
{
    public abstract class Foodbed : FoodObject, INotificationProvider
    {
        [SerializeField] protected Scriptable.FoodBed scriptable;

        private SpriteRenderer spriteRenderer;
        private new ParticleSystem particleSystem;

        public FoodSpawnPoint FoodSpawnPoint { get; set; }
        public virtual bool CanGrow => Amount < scriptable.MaxAmount;
        public override int StartAmount => scriptable.GetRandomAmount();
        public override FoodType FoodType => scriptable.FoodType;


        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        protected override void Start()
        {
            base.Start();
            
            if (particleSystem is not null)
            {
                var main = particleSystem.main;
                main.startColor = spriteRenderer.color;
            }

            UpdateSprite();
            if (CreateNotification) GlobalDefinitions.CreateNotification(this);
        }

        protected override void OnEaten()
        {
            OnDataUpdate?.Invoke();
            UpdateSprite();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            OnProviderDestroy?.Invoke(this);
            if (FoodSpawnPoint is not null)
                FoodSpawnPoint.Clear();
        }


        public void Grow()
        {
            if (!CanGrow) return;

            Amount++;
            OnDataUpdate?.Invoke();
            UpdateSprite();
        }

        protected virtual Sprite GetGrowthSprite(int amount) => scriptable.GetGrowthSprite(amount - 1);
        protected void UpdateSprite() => spriteRenderer.sprite = GetGrowthSprite(Amount);
        

        public virtual bool CanSpawn(float random)
        {
            return random <= scriptable.SpawnChance &&
                   TimeManager.DayCounter > scriptable.CanSpawnFromDay &&
                   TimeManager.Is(scriptable.TimeOfDay);
        }


        // IContinuouslyInteractable
        public override void OnInteractionStart()
        {
            if (particleSystem is not null)
                particleSystem.Play();
        }
        
        public override float PopupDistance => 1.25f;


        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => Amount.ToString();
        protected virtual bool CreateNotification => true;
    }
}