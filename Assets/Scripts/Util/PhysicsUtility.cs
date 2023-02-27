using Definitions;
using Player;
using UnityEngine;

namespace Util
{
    public static class PhysicsUtility
    {
        public static Vector2 GetVelocityBackwards(Vector2 victim, Vector2 attacker, float knockbackPower) 
            => (victim - attacker).normalized * knockbackPower;
        
        public static void RotateTowardsPosition(this Rigidbody2D rb, Vector2 targetPos, float delta) 
            => rb.rotation = RotationTowards(rb.position, rb.rotation, targetPos, delta);

        public static float RotationTowards(Vector2 pos, float rot, Vector2 targetPos, float delta)
        {
            Vector2 direction = targetPos - pos;
            float angle = direction.GetAngle() - 90f;
            return Mathf.MoveTowardsAngle(rot, angle, delta);
        }

        public static float GetAngle(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        
        public static float CalculateDamage(float incomingDamage, float armor) 
            => incomingDamage * incomingDamage / (incomingDamage + armor);

        public static void AddClampedForceTowards
            (this Rigidbody2D rb, 
            Vector2 direction, 
            float force, 
            ForceMode2D mode, 
            float maxAmplifier = 1) 
                => rb.AddForce(
                    (direction - rb.position).normalized * 
                    Mathf.Clamp(force, 0, GlobalDefinitions.MaxAppliableForce * maxAmplifier) * 
                    rb.mass / GlobalDefinitions.PlayerMass, mode);
        
        public static void AddClampedForceBackwards
            (this Rigidbody2D rb, 
            Vector2 direction, 
            float force, 
            ForceMode2D mode,
            float maxAmplifier = 1) 
                => rb.AddForce(
                    -1 *
                    (direction - rb.position).normalized * 
                    Mathf.Clamp(force, 0, GlobalDefinitions.MaxAppliableForce * maxAmplifier) * 
                    rb.mass / GlobalDefinitions.PlayerMass, mode);

        public static float GetKnockbackResistance(float playerMass)
            => Mathf.InverseLerp(0, GlobalDefinitions.PlayerMass * 2, playerMass) - 0.1f;
    }
}