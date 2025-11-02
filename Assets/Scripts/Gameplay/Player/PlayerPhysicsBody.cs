using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerPhysicsBody : MonoBehaviour
    {
        private static RigidbodyConstraints2D defaultConstraints;
        public static Rigidbody2D Rigidbody { get; private set; }
        public static Collider2D PhysicsCollider { get; private set; }
        public static Vector2 Position => Rigidbody.position;
        public static float Rotation => Rigidbody.rotation;
        
        
        
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            PhysicsCollider = GetComponent<Collider2D>();
            defaultConstraints = Rigidbody.constraints;
        }

        public static void ResetConstraints() => Rigidbody.constraints = defaultConstraints;
    }
}