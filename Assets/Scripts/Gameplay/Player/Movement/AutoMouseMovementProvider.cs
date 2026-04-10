using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class AutoMouseMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result, out ForceMode2D forceMode)
        {
            result = default;
            forceMode = ForceMode2D.Force;
            if (Interaction.Interactor.Interacting) return false;

            result = playerTransform.up;

            return true;
        }
    }
}