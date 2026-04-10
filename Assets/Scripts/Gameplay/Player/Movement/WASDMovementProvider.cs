using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class WASDMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            result = new Vector2(x, y).normalized;

            return x != 0f || y != 0f;
        }
    }
}