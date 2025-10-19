using Gameplay.Effects.WildCucumber;
using Gameplay.Player;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class Cucumberbardier : ActiveAbility
    {
        // TODO: explosion damage, ability description & icon
        [Header("Spontaneous Explosion Chance")] 
        [SerializeField, Range(0, 1)] protected float explosionChanceLv1;
        [SerializeField, Range(0, 1)] protected float explosionChanceLv10;
        [Header("Seeds Amount")] 
        [SerializeField, Range(1, 20)] private int seedsAmountLvl1;
        [SerializeField, Range(1, 20)] private int seedsAmountLvl10;
        [Header("Seeds Damage")] 
        [SerializeField, Range(0.01f, 5)] private float seedsDamageLvl1;
        [SerializeField, Range(0.01f, 5)] private float seedsDamageLvl10;
        [Header("Knockback Power")] 
        [SerializeField, Range(0.01f, 5)] private float knockbackLvl1;
        [SerializeField, Range(0.01f, 5)] private float knockbackLvl10;
        [Header("Projectile Power")] 
        [SerializeField, Range(1f, 20f)] private float projectilePowerLv1;
        [SerializeField, Range(1f, 20f)] private float projectilePowerLv10;
        [Header("Decay Time")]
        [SerializeField] private float decayTime;
        
        
        private float explosionChance;
        private int seedsAmount;
        private float seedsDamage;
        private float knockback;
        private float projectilePower;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            explosionChance = LerpLevel(explosionChanceLv1, explosionChanceLv10, lvl);
            seedsAmount = LerpLevel(seedsAmountLvl1, seedsAmountLvl10, lvl);
            seedsDamage = LerpLevel(seedsDamageLvl1, seedsDamageLvl10, lvl);
            knockback = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            projectilePower = LerpLevel(projectilePowerLv1, projectilePowerLv10, lvl);
        }

        public override void Activate()
        {
            PoolManager.GetEffect<WildCucumberProjectile>(new WildCucumberArguments(
                explosionChance, seedsAmount, 
                seedsDamage, knockback, 
                PlayerManager.Instance.Transform.up, 
                projectilePower, decayTime
                ), position: transform.position);
        }

        public override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return default;
        }
    }
}