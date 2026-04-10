using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class MouseSwimProvider : PlayerMovementProvider
    {
        [SerializeField, Range(0.1f, 1f)] private float step;

        public override ForceMode2D ForceMode => ForceMode2D.Impulse;

        public override bool ProvideMovement(Transform playerTransform, out Vector2 result)
        {
            // TODO: this will bypass frame check in Whirlwind movement
            result = playerTransform.up;
            if (!Input.GetMouseButton(1)) return false;

            result *= step;
            float t = Time.fixedTime % step;
            return Mathf.Abs(step - t) <= Time.fixedDeltaTime;
        }
    }
}