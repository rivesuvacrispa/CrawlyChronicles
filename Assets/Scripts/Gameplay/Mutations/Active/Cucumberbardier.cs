using Gameplay.Effects.WildCucumber;
using Gameplay.Player;
using Pooling;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class Cucumberbardier : ActiveAbility
    {
        [SerializeField, MinMaxRange(0, 1)] private LevelFloat explosionChance = new LevelFloat(new Vector2(0.05f, 0.2f));
        [SerializeField, MinMaxRange(1f, 10f)] private LevelFloat explosionDamage = new LevelFloat(new Vector2(2f, 10f));
        [SerializeField, MinMaxRange(1f, 5f)] private LevelFloat explosionRange = new LevelFloat(new Vector2(1f, 2.25f));
        [SerializeField, MinMaxRange(0.0f, 10f)] private LevelFloat knockback = new LevelFloat(new Vector2(2f, 10f));
        [SerializeField, MinMaxRange(1, 20)] private LevelInt seedsAmount = new LevelInt(new Vector2Int(5, 15));
        [SerializeField, MinMaxRange(0.01f, 5f)] private LevelFloat seedsDamage = new LevelFloat(new Vector2(1f, 5f));
        [SerializeField, MinMaxRange(1f, 20f)] private LevelFloat projectilePower = new LevelFloat(new Vector2(5f, 10f));

        private float currentExplosionChance;
        private int currentSeedsAmount;
        private float currentSeedsDamage;
        private float currentKnockback;
        private float currentProjectilePower;
        private float currentExplosionDamage;
        private float currentExplosionRange;

        
        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);

            currentExplosionChance = explosionChance.AtLvl(lvl);
            currentSeedsAmount = seedsAmount.AtLvl(lvl);
            currentSeedsDamage = seedsDamage.AtLvl(lvl);
            currentKnockback = knockback.AtLvl(lvl);
            currentProjectilePower = projectilePower.AtLvl(lvl);
            currentExplosionDamage = explosionDamage.AtLvl(lvl);
            currentExplosionRange = explosionRange.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            var cucumber = PoolManager.GetEffect<WildCucumberProjectile>(new WildCucumberArguments(
                currentExplosionChance, currentSeedsAmount, 
                currentSeedsDamage, currentKnockback, 
                PlayerManager.Instance.Transform.up, 
                currentProjectilePower,
                currentExplosionDamage, currentExplosionRange, instant: AttackController.IsInComboDash
            ), position: transform.position);
            
            if (AttackController.IsInComboDash)
                cucumber.Explode();
        }

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                explosionChance.UseKey(LevelFieldKeys.EXPLOSION_CHANCE).UseFormatter(StatFormatter.PERCENT),
                seedsAmount.UseKey(LevelFieldKeys.SEEDS_AMOUNT),
                seedsDamage.UseKey(LevelFieldKeys.SEEDS_DAMAGE),
                knockback.UseKey(LevelFieldKeys.KNOCKBACK),
                projectilePower.UseKey(LevelFieldKeys.THROW_POWER),
                explosionDamage.UseKey(LevelFieldKeys.EXPLOSION_DAMAGE),
                explosionRange.UseKey(LevelFieldKeys.EXPLOSION_RANGE),
            };
        }
    }
}