using Gameplay.AI.Locators;
using Gameplay.Interaction;
using UI;
using UnityEngine;

namespace Gameplay.Food
{
    public abstract class FoodBed : MonoBehaviour, ILocatorTarget, IFoodBed, IContinuouslyInteractable
    {
        public abstract void Eat();
        
        
        private void Start() => MainMenu.OnResetRequested += OnResetRequested;

        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnResetRequested() => Destroy(gameObject);
        
        
        // IContinuouslyInteractable
        public void Interact()
        {
            Eat();
            Player.Manager.Instance.AddHealth(1);
            BreedingManager.Instance.AddFood();
        }
        
        public abstract void OnInteractionStart();
        public abstract void OnInteractionStop();
        public abstract bool CanInteract();
        public float InteractionTime => 1.5f;
        public float PopupDistance => 1.25f;
        public string ActionTitle => "Eat";
        public Vector3 Position => transform.position;
    }
}