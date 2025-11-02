using Gameplay.Effects.WildCucumber;
using Gameplay.Player;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class Cucumberbardier : ActiveAbility
    {
        // TODO: ability description & icon
        [Header("Spontaneous Explosion Chance")] 
        [SerializeField, Range(0, 1)] protected float explosionChanceLv1;
        [SerializeField, Range(0, 1)] protected float explosionChanceLv10;
        [Header("Explosion Damage")] 
        [SerializeField, Range(1f, 10f)] private float explosionDamageLvl1;
        [SerializeField, Range(1f, 10f)] private float explosionDamageLvl10;
        [Header("Explosion Range")] 
        [SerializeField, Range(1f, 5f)] private float explosionRangeLvl1;
        [SerializeField, Range(1f, 5f)] private float explosionRangeLvl10;
        [Header("Explosion Power")] 
        [SerializeField, Range(0.01f, 10f)] private float knockbackLvl1;
        [SerializeField, Range(0.01f, 10f)] private float knockbackLvl10;
        [Header("Seeds Amount")] 
        [SerializeField, Range(1, 20)] private int seedsAmountLvl1;
        [SerializeField, Range(1, 20)] private int seedsAmountLvl10;
        [Header("Seeds Damage")] 
        [SerializeField, Range(0.01f, 5)] private float seedsDamageLvl1;
        [SerializeField, Range(0.01f, 5)] private float seedsDamageLvl10;
        [Header("Projectile Power")] 
        [SerializeField, Range(1f, 20f)] private float projectilePowerLv1;
        [SerializeField, Range(1f, 20f)] private float projectilePowerLv10;
        
        
        private float explosionChance;
        private int seedsAmount;
        private float seedsDamage;
        private float knockback;
        private float projectilePower;
        private float explosionDamage;
        private float explosionRange;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            explosionChance = LerpLevel(explosionChanceLv1, explosionChanceLv10, lvl);
            seedsAmount = LerpLevel(seedsAmountLvl1, seedsAmountLvl10, lvl);
            seedsDamage = LerpLevel(seedsDamageLvl1, seedsDamageLvl10, lvl);
            knockback = LerpLevel(knockbackLvl1, knockbackLvl10, lvl);
            projectilePower = LerpLevel(projectilePowerLv1, projectilePowerLv10, lvl);
            explosionDamage = LerpLevel(explosionDamageLvl1, explosionDamageLvl10, lvl);
            explosionRange = LerpLevel(explosionRangeLvl1, explosionRangeLvl10, lvl);
        }

        public override void Activate()
        {
            base.Activate();
            var cucumber = PoolManager.GetEffect<WildCucumberProjectile>(new WildCucumberArguments(
                explosionChance, seedsAmount, 
                seedsDamage, knockback, 
                PlayerManager.Instance.Transform.up, 
                projectilePower,
                explosionDamage, explosionRange, instant: AttackController.IsInComboDash
            ), position: transform.position);
            
            if (AttackController.IsInComboDash)
                cucumber.Explode();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return default;
        }
    }
}