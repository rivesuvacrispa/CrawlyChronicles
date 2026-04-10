using UnityEngine;

namespace Gameplay.Player.Movement
{
    public abstract class PlayerMovementProvider : MonoBehaviour
    {
        public abstract bool ProvideMovement(Transform playerTransform, out Vector2 result, out ForceMode2D forceMode);
    }
}