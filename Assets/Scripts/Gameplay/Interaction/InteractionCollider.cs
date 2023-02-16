using UnityEngine;

namespace Gameplay.Interaction
{
    public class InteractionCollider : MonoBehaviour
    {
        public IInteractable Interactable { get; private set; }

        private void Awake() => Interactable = GetComponentInParent<IInteractable>();
    }
}