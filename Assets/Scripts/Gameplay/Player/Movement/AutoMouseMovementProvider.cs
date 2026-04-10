using UnityEngine;

namespace Gameplay.Player.Movement
{
    public class AutoMouseMovementProvider : PlayerMovementProvider
    {
        public override bool ProvideMovement(Transform playerTransform, out Vector2 result)
        {
            result = playerTransform.up;
            
            return !Interaction.Interactor.Interacting;
        }
    }
}