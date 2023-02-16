using Gameplay.AI.Locators;
using Gameplay.Interaction;
using UnityEngine;

namespace Gameplay.Food
{
    public abstract class FoodBed : MonoBehaviour, ILocatorTarget, IFoodBed, IInteractable
    {
        public abstract void Eat();
        
        
        
        // IInteractable
        public void Interact()
        {
            Eat();
            Player.Manager.Instance.AddHealth(1);
            BreedingManager.Instance.AddFood();
        }

        public abstract bool CanInteract();
        public float InteractionTime => 2f;
        public float PopupDistance => 1.25f;
        public string ActionTitle => "Eat";
        public Vector3 Position => transform.position;
    }
}