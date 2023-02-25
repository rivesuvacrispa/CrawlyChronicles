using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Interaction;
using UI;
using UnityEngine;
using Util;

namespace Gameplay.Food
{
    public abstract class FoodBed : MonoBehaviour, ILocatorTarget, IFoodBed, IContinuouslyInteractable, INotificationProvider
    {
        [SerializeField] protected Scriptable.FoodBed scriptable;
        
        private int amount;

        public FoodSpawnPoint FoodSpawnPoint { get; set; }

        private void Start()
        {
            MainMenu.OnResetRequested += OnResetRequested;
            amount = scriptable.GetRandomAmount();
            GlobalDefinitions.CreateNotification(this);
        }

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
            OnProviderDestroy?.Invoke();
        }
        
        public void Eat()
        {
            amount--;
            if (amount <= 0)
            {
                if(FoodSpawnPoint is null) Destroy(gameObject);
                else FoodSpawnPoint.Remove();
            }
            else OnDataUpdate?.Invoke();
        }

        protected abstract void OnEatenByPlayer();

        private void OnResetRequested() => FoodSpawnPoint.Remove();



        // IContinuouslyInteractable
        public void Interact()
        {
            Eat();
            BreedingManager.Instance.AddFood();
            OnEatenByPlayer();
        }

        public void OnInteractionStart() { }
        public void OnInteractionStop() { }
        public bool CanInteract() => true;
        public float InteractionTime => 1.5f;
        public float PopupDistance => 1.25f;
        public string ActionTitle => "Eat";
        public Vector3 Position => transform.position;
        
        
        
        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.DestructionProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => amount.ToString();
    }
}