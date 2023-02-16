using System.Collections;
using UI;
using UnityEngine;

namespace Gameplay.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private InteractionPopup popup;
        [SerializeField] private float popupDistance;
        
        private IInteractable interactable;
        private bool canInteract;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
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
                if (interactable.InteractionTime == 0)
                {
                    interactable.Interact();
                    popup.Disable();
                }
                else StartCoroutine(InteractionRoutine());
            }
        }

        private IEnumerator InteractionRoutine()
        {
            if (interactable is null) yield break;
            
            popup.SetFilling(0);
            float duration = interactable.InteractionTime;
            bool interrupted = false;
            float interactionTime = 0;
            while (interactionTime < duration)
            {
                if (Input.GetKeyUp(KeyCode.E) || !canInteract || interactable is null)
                {
                    interrupted = true;
                    break;
                }
                popup.SetFilling(Mathf.Clamp01(interactionTime / interactable.InteractionTime));
                interactionTime += Time.deltaTime;
                yield return null;
            }
            
            if(!interrupted)
            {
                interactable.Interact();
                popup.Disable();
                StartCoroutine(InteractionRoutine());
            } else popup.SetFilling(0);
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