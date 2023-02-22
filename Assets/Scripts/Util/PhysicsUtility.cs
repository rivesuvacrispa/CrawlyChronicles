using Unity.Mathematics;
using UnityEngine;

namespace Util
{
    public static class PhysicsUtility
    {
        public static Vector2 GetKnockbackVelocity(Vector2 victim, Vector2 attacker, float knockbackPower) 
            => (victim - attacker).normalized * knockbackPower;
        
        public static float RotateTowardsPosition(Vector2 originPos, float originRotation, Vector2 targetPos, float delta)
        {
            Vector2 rotateDirection = targetPos - originPos;
            float angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) - Mathf.PI * 0.5f;
            return RotateTowardsAngle(originRotation, angle, delta);
        }

        private static float RotateTowardsAngle(float rotation, float angle, float delta)
        {
            return Quaternion.RotateTowards(
                Quaternion.Euler(0, 0, rotation), 
                quaternion.Euler(0, 0, angle),
                delta).eulerAngles.z;
        }

        public static float CalculateDamage(float incomingDamage, float armor)
            => incomingDamage * incomingDamage / (incomingDamage + armor);
    }
}