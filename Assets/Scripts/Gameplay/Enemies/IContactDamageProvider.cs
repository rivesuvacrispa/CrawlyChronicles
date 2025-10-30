using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Enemies
{
    public interface IContactDamageProvider : IDamageSource
    {
        public float ContactDamage { get; }
        public Vector3 ContactDamagePosition { get; }
        public float ContactDamageKnockback { get; }
        public float ContactDamageStunDuration { get; }
        public Color ContactDamageColor { get; }
        public bool ContactDamagePiercing { get; }
    }
}