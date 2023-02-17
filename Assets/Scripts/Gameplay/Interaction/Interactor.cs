using System.Collections;
using UI;
using UnityEngine;

namespace Gameplay.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private InteractionPopup popup;
        
        private IInteractable interactable;
        private bool canInteract;
        
        public static bool Interacting { get; private set; }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(Interacting) return;
            if(col.TryGetComponent(out InteractionCollider iCol)) interactable = iCol.Interactable;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if(interactable is null) return;

            if (col.TryGetComponent(out InteractionCollider iCol) && interactable == iCol.Interactable)
                interactable = null;
        }

        private void Update()
        {
            if (interactable is null)
            {
                popup.Disable();
                return;
            }

            canInteract = interactable.CanInteract() &&
                               Player.Manager.Instance.AllowInteract;
            
            if(canInteract) UpdatePopup();
            else popup.Disable();

            if(Input.GetKeyDown(KeyCode.E) && canInteract)
            {
                if (interactable is IContinuouslyInteractable continuouslyInteractable)
                    StartCoroutine(InteractionRoutine(continuouslyInteractable));
                else
                {
                    interactable.Interact();
                    popup.Disable();
                }
            }
        }

        private IEnumerator InteractionRoutine(IContinuouslyInteractable continuouslyInteractable)
        {
            if (interactable is null) yield break;

            Interacting = true;
            continuouslyInteractable.OnInteractionStart();
            popup.SetFilling(0);
            float duration = continuouslyInteractable.InteractionTime;
            bool interrupted = false;
            float interactionTime = 0;
            while (interactionTime < duration)
            {
                if (Input.GetKeyUp(KeyCode.E) || !canInteract || interactable is null)
                {
                    interrupted = true;
                    break;
                }
                popup.SetFilling(Mathf.Clamp01(interactionTime / duration));
                interactionTime += Time.deltaTime;
                yield return null;
            }
            
            if(!interrupted)
            {
                interactable.Interact();
                popup.Disable();
                StartCoroutine(InteractionRoutine(continuouslyInteractable));
            } else
            {
                popup.SetFilling(0);
                continuouslyInteractable.OnInteractionStop();
                Interacting = false;
            }
        }

        private void UpdatePopup()
        {
            Vector2 interactablePos = interactable.Position;
            Vector2 pos = interactablePos + (interactablePos - Player.Movement.Position).normalized * interactable.PopupDistance;
            popup.transform.position = pos;
            popup.Enable(interactable.ActionTitle, KeyCode.E);
        }
    }
}