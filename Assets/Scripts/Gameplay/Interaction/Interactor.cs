using System.Collections;
using SoundEffects;
using UI;
using UnityEngine;

namespace Gameplay.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private InteractionPopup popup;

        private static IInteractable interactable;

        public static bool CanInteract () => interactable is not null &&
                                             interactable.CanInteract() &&
                                             Player.PlayerManager.Instance.AllowInteract;
        public static bool Interacting { get; private set; }


        public static void Abort()
        {
            interactable = null;
            Interacting = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (Interacting) return;
            if (col.TryGetComponent(out InteractionCollider iCol)) interactable = iCol.Interactable;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (interactable is null) return;

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

            bool canInteract = CanInteract(); 

            if (canInteract) UpdatePopup();
            else popup.Disable();

            if (Input.GetKeyDown(KeyCode.E) && canInteract)
            {
                if (interactable is IContinuouslyInteractable continuouslyInteractable
                    && continuouslyInteractable.InteractionTime != 0)
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
            PlayerAudioController.Instance.PlayInteract();
            continuouslyInteractable.OnInteractionStart();
            popup.SetFilling(0);
            float duration = continuouslyInteractable.InteractionTime;
            bool interrupted = false;
            float interactionTime = 0;

            while (interactionTime < duration)
            {
                if (Input.GetKeyUp(KeyCode.E) || !CanInteract() || interactable is null)
                {
                    interrupted = true;
                    break;
                }

                popup.SetFilling(Mathf.Clamp01(interactionTime / duration));
                interactionTime += Time.deltaTime;
                yield return null;
            }

            popup.SetFilling(0);
            continuouslyInteractable.OnInteractionStop();
            PlayerAudioController.Instance.StopAction();
            Interacting = false;

            if (!interrupted)
            {
                interactable.Interact();
                popup.Disable();
                StartCoroutine(InteractionRoutine(continuouslyInteractable));
            }
        }

        private void UpdatePopup()
        {
            Vector2 interactablePos = interactable.Position;
            Vector2 pos = interactablePos + (interactablePos - Player.PlayerMovement.Position).normalized *
                interactable.PopupDistance;
            popup.transform.position = pos;
            popup.Enable(interactable.ActionTitle, KeyCode.E);
        }
    }
}