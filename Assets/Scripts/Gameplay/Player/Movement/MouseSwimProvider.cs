using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class MouseSwimProvider : PlayerMovementProvider
    {
        [SerializeField, Range(0.1f, 1f)] private float step;
        
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result, out ForceMode2D forceMode)
        {
            result = playerTransform.up;
            forceMode = ForceMode2D.Impulse;
            if (!Input.GetMouseButton(1)) return false;

            result *= step;
            float t = Time.fixedTime % step;
            print($"FDT: {Time.fixedTime}, t % {step} = {t}");
            return Mathf.Abs(step - t) <= Time.fixedDeltaTime;
        }
    }
}