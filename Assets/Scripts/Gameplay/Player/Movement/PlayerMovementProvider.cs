using UnityEngine;

namespace Gameplay.Player.Movement
{
    public abstract class PlayerMovementProvider : MonoBehaviour
    {
        public virtual ForceMode2D ForceMode => ForceMode2D.Force;
        public abstract bool ProvideMovement(Transform playerTransform, out Vector2 result);
    }
}