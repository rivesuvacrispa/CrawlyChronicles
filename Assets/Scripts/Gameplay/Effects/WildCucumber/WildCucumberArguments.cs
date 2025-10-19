using UnityEngine;

namespace Gameplay.Effects.WildCucumber
{
    public readonly struct WildCucumberArguments
    {
        public readonly float explosionChance;
        public readonly int seedsAmount;
        public readonly float seedsDamage;
        public readonly float knockback;
        public readonly Vector3 direction;
        public readonly float projectilePower;
        public readonly float decayTime;

        public WildCucumberArguments(float explosionChance, int seedsAmount, float seedsDamage, float knockback, Vector3 direction, float projectilePower, float decayTime)
        {
            this.explosionChance = explosionChance;
            this.seedsAmount = seedsAmount;
            this.seedsDamage = seedsDamage;
            this.knockback = knockback;
            this.direction = direction;
            this.projectilePower = projectilePower;
            this.decayTime = decayTime;
        }
    }
}