using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Interaction;
using Genes;
using UI;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Food
{
    public abstract class Foodbed : MonoBehaviour, ILocatorTarget, IFoodBed, IContinuouslyInteractable, INotificationProvider
    {
        [SerializeField] protected Scriptable.FoodBed scriptable;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new ParticleSystem particleSystem;
        
        public int Amount { get; protected set; }
        private Animator animator;
        private bool destructionInvoked;

        public FoodSpawnPoint FoodSpawnPoint { get; set; }
        private static readonly int PopoutAnimHash = Animator.StringToHash("FoodbedPopout");
        private static readonly int PopAnimHash = Animator.StringToHash("FoodbedPop");


        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            Amount = scriptable.GetRandomAmount();
            var main = particleSystem.main;
            main.startColor = spriteRenderer.color;
            UpdateSprite();
            if(CreateNotification) GlobalDefinitions.CreateNotification(this);
        }

        protected virtual void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            if(!destructionInvoked) OnProviderDestroy?.Invoke();
        }
        
        public bool Eat()
        {
            if (Amount <= 0) return false;
            
            Amount--;
            if (Amount <= 0)
            {
                animator.Play(PopoutAnimHash);
                OnProviderDestroy?.Invoke();
                destructionInvoked = true;
                if(FoodSpawnPoint is not null) FoodSpawnPoint.Clear();
            }
            else
            {
                animator.Play(PopAnimHash);
                OnDataUpdate?.Invoke();
                UpdateSprite();
            }

            return true;
        }
        
        private void UpdateSprite() => spriteRenderer.sprite = scriptable.GetGrowthSprite(Amount);
        private void OnResetRequested()
        {
            FoodSpawnPoint.Clear();
            Destroy(gameObject);
        }

        protected abstract void OnEatenByPlayer();
        public abstract bool CanSpawn(float random);

        

        // IContinuouslyInteractable
        public void Interact()
        {
            if (Eat())
            {
                if(!BreedingManager.Instance.AddFood())
                    GlobalDefinitions.DropGenesRandomly(Position, (GeneType)Random.Range(0, 3), 1, 0.4f);
                OnEatenByPlayer();
            }
        }

        public void OnInteractionStart() => particleSystem.Play();
        public void OnInteractionStop() => particleSystem.Stop();
        public virtual bool CanInteract() => Amount > 0;
        public float InteractionTime => 1f;
        public float PopupDistance => 1.25f;
        public string ActionTitle => "Eat";
        public virtual Vector3 Position => transform.position;
        
        
        
        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => Amount.ToString();
        protected virtual bool CreateNotification => true;
    }
}