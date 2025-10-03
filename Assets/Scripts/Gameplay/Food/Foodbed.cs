using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using Definitions;
using DG.Tweening;
using Gameplay.AI.Locators;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Interaction;
using UI.Menus;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Food
{
    public abstract class Foodbed : MonoBehaviour, ILocatorTarget, IFoodBed, IContinuouslyInteractable, INotificationProvider
    {
        [SerializeField] protected Scriptable.FoodBed scriptable;
        
        public int Amount { get; protected set; }
        private bool destructionInvoked;
        private SpriteRenderer spriteRenderer;
        private new ParticleSystem particleSystem;

        public FoodSpawnPoint FoodSpawnPoint { get; set; }
        private const float ANIMATION_DURATION = 0.25f;


        private void Awake()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            particleSystem = GetComponentInChildren<ParticleSystem>();
            transform.localScale = Vector3.zero;
        }

        protected virtual void Start()
        {
            Amount = scriptable.GetRandomAmount();
            var main = particleSystem.main;
            main.startColor = spriteRenderer.color;
            UpdateSprite();
            if(CreateNotification) GlobalDefinitions.CreateNotification(this);
            PlayPopIn(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        protected virtual void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            if(!destructionInvoked) OnProviderDestroy?.Invoke(this);
        }

        private async UniTask PlayPopIn(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.75f))
                .Append(transform.DOScale(Vector3.one, ANIMATION_DURATION * 0.25f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }

        private async UniTask PlayPopOut(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.25f))
                .Append(transform.DOScale(Vector3.zero, ANIMATION_DURATION * 0.75f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            
            Destroy(gameObject);
        }
        
        private async UniTask PlayPop(CancellationToken cancellationToken)
        {
            await DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 0.75f, ANIMATION_DURATION * 0.2f))
                .Append(transform.DOScale(Vector3.one * 1.25f, ANIMATION_DURATION * 0.5f))
                .Append(transform.DOScale(Vector3.one, ANIMATION_DURATION * 0.3f))
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }
        
        public bool Eat()
        {
            if (Amount <= 0) return false;
            
            Amount--;
            if (Amount <= 0)
            {
                PlayPopOut(gameObject.GetCancellationTokenOnDestroy()).Forget();
                OnProviderDestroy?.Invoke(this);
                destructionInvoked = true;
                if(FoodSpawnPoint is not null) FoodSpawnPoint.Clear();
            }
            else
            {
                PlayPop(gameObject.GetCancellationTokenOnDestroy()).Forget();
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

        public void OnInteractionStop()
        {
        }

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