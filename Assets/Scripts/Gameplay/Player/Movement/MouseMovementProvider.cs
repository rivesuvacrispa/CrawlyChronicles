using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class MouseMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result)
        {
            result = playerTransform.up;
            
            return Input.GetMouseButton(1);
        }
    }
}