using UnityEngine;

namespace Gameplay.Interaction
{
    public interface IInteractable
    {
        public void Interact();
        public bool CanInteract();
        
        public float InteractionTime { get; }
        public float PopupDistance { get; }
        public string ActionTitle { get; }
        public Vector3 Position { get; }
    }
}