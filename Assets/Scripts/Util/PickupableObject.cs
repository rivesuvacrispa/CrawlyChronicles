using System.Collections;
using Definitions;
using UI;
using UI.Menus;
using UnityEngine;

namespace Util
{
    public abstract class PickupableObject : MonoBehaviour
    {
        protected abstract void OnPickup();
        
        private void Start()
        {
            StartCoroutine(SpawnRoutine());
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void Update()
        {
            Vector2 direction = Player.PlayerMovement.Position - (Vector2) transform.position;
            float distanceFromPlayer = direction.sqrMagnitude;
            float pickUpDistance = GlobalDefinitions.GenePickupDistance;
            if (distanceFromPlayer <= pickUpDistance)
            {
                enabled = false;
                StartCoroutine(ConsumingRoutine(pickUpDistance, distanceFromPlayer));
            }
        }

        private IEnumerator ConsumingRoutine(float pickUpDistance, float distanceFromPlayer)
        {
            while (distanceFromPlayer >= 0.15f)
            {
                Vector2 pos = transform.position;
                Vector2 playerpos = Player.PlayerMovement.Position;
                Vector2 direction = playerpos - pos;
                distanceFromPlayer = direction.sqrMagnitude;
                transform.position = Vector2.MoveTowards(pos, playerpos,
                    Time.deltaTime * (pickUpDistance / distanceFromPlayer));
                yield return null;
            }

            OnPickup();
            Destroy(gameObject);
        }

        private IEnumerator SpawnRoutine()
        {
            enabled = false;
            yield return new WaitForSeconds(0.75f);
            enabled = true;
        }
        
        private void OnDestroy() => MainMenu.OnResetRequested -= OnResetRequested;

        private void OnResetRequested() => Destroy(gameObject);
    }
}