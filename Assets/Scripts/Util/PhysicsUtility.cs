using System;
using Definitions;
using UnityEngine;

namespace Util
{
    public static class PhysicsUtility
    {
        public static Vector2 GetVelocityBackwards(Vector2 victim, Vector2 attacker, float velocity) 
            => (victim - attacker).normalized * velocity;
        
        public static Vector2 GetVelocityTowards(Vector2 victim, Vector2 attacker, float velocity) 
            => (attacker - victim).normalized * velocity;
        
        public static void RotateTowardsPosition(this Rigidbody2D rb, Vector2 targetPos, float delta) 
            => rb.rotation = RotationTowards(rb.position, rb.rotation, targetPos, delta);

        public static float RotationTowards(Vector2 pos, float rot, Vector2 targetPos, float delta)
        {
            Vector2 direction = targetPos - pos;
            float angle = direction.GetAngle() - 90f;
            return Mathf.MoveTowardsAngle(rot, angle, delta);
        }

        public static bool AngleBetween(Vector2 a, Vector2 b, float angle)
        {
            float aa = a.GetAngle();
            float bb = b.GetAngle();
            aa *= aa;
            bb *= bb;
            return Math.Abs(aa - bb) <= angle * angle;
        }

        public static float GetAngle(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        
        public static float CalculateDamage(float incomingDamage, float armor)
        {
            if (incomingDamage is float.NaN or 0)
                return 0;

            if (armor <= 0f)
                return incomingDamage;
            
            return incomingDamage * incomingDamage / (incomingDamage + armor);
        }

        public static void AddClampedForceTowards
            (this Rigidbody2D rb, 
            Vector2 direction, 
            float force, 
            ForceMode2D mode, 
            float maxAmplifier = 1) 
                => rb.AddForce(
                    (direction - rb.position).normalized * 
                    (Mathf.Clamp(force, 0, GlobalDefinitions.MaxAppliableForce * maxAmplifier) * rb.mass) / 
                    GlobalDefinitions.UnitMass, mode);
        
        public static void AddClampedForceBackwards
            (this Rigidbody2D rb, 
            Vector2 direction, 
            float force, 
            ForceMode2D mode,
            float maxAmplifier = 1) 
                => rb.AddForce(
                    (direction - rb.position).normalized * 
                    (-1 * Mathf.Clamp(force, 0, GlobalDefinitions.MaxAppliableForce * maxAmplifier) * rb.mass) 
                    / GlobalDefinitions.UnitMass, mode);

        public static float GetKnockbackResistance(float mass)
            => Mathf.InverseLerp(0, GlobalDefinitions.UnitMass * 2, mass);
    }
}