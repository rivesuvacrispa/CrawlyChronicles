using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class MouseMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result, out ForceMode2D forceMode)
        {
            result = default;
            forceMode = ForceMode2D.Force;
            if (!Input.GetMouseButton(1)) return false;
            
            result = playerTransform.up;
            return true;
        }
    }
}