using Gameplay.Player;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class DigSprite : MonoBehaviour
    {
        private void Awake()
        {
            transform.SetParent(null);
            transform.rotation = Quaternion.identity;
            MainMenu.OnResetRequested += OnResetRequested;
        }

        private void OnDestroy()
        {
            MainMenu.OnResetRequested -= OnResetRequested;
        }

        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            transform.position = PlayerPhysicsBody.Position;
        }

        private void OnResetRequested() => gameObject.SetActive(false);
    }
}