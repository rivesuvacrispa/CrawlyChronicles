using Gameplay.Player;
using UI.Menus;
using UnityEngine;

namespace Util.Components
{
    public class DigSprite : MonoBehaviour
    {
        private void Awake()
        {
            Transform t = transform;
            t.SetParent(null);
            t.rotation = Quaternion.identity;
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