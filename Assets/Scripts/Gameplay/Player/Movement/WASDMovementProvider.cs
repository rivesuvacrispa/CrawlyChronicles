using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class WASDMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result, out ForceMode2D forceMode)
        {
            result = default;
            forceMode = ForceMode2D.Force;
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x == 0f && y == 0f) return false;

            result = new Vector2(x, y).normalized;
            print(result);
            return true;
        }
    }
}