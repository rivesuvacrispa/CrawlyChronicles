using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;
using Util.Abilities;

namespace Gameplay.Mutations.Stats
{
    public class StatsAbility : BasicAbility
    {
        [SerializeField] private PlayerStats statsLvl1;
        [SerializeField] private PlayerStats statsLvl10;

        [SerializeField] protected PlayerStats current = PlayerStats.Zero;

        protected bool StatsCanBeAdded =>
            !current.Equals(PlayerStats.Zero) && Application.isPlaying && isActiveAndEnabled;

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            if (StatsCanBeAdded) PlayerManager.Instance.AddStats(current.Negated());
            current = PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl);
            if (StatsCanBeAdded) PlayerManager.Instance.AddStats(current);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (StatsCanBeAdded)
                PlayerManager.Instance.AddStats(current);
        }
        
        protected override void OnDisable()
        {
            if (StatsCanBeAdded)
                PlayerManager.Instance.AddStats(current.Negated());
            base.OnDisable();
        }

        /*public override string GetLevelDescription(int lvl, bool withUpgrade)
        {
            var selfStats = PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl);
            return selfStats.PrintCompared(
                lvl == 0 || !withUpgrade
                    ? selfStats
                    : PlayerStats.LerpLevel(statsLvl1, statsLvl10, lvl - 1));
        }*/

        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            List<ILevelField> fields = new List<ILevelField>();
            
            if(statsLvl1.MovementSpeed != 0 || statsLvl10.MovementSpeed != 0)
                fields.Add(new LevelFloat(statsLvl1.MovementSpeed, statsLvl10.MovementSpeed).UseKey(LevelFieldKeys.MOVEMENT_SPEED));
            
            if(statsLvl1.RotationSpeed != 0 || statsLvl10.RotationSpeed != 0)
                fields.Add(new LevelFloat(statsLvl1.RotationSpeed, statsLvl10.RotationSpeed).UseKey(LevelFieldKeys.ROTATION_SPEED));
            
            if(statsLvl1.MaxHealth != 0 || statsLvl10.MaxHealth != 0)
                fields.Add(new LevelFloat(statsLvl1.MaxHealth, statsLvl10.MaxHealth).UseKey(LevelFieldKeys.MAX_HEALTH));
            
            if(statsLvl1.AttackPower != 0 || statsLvl10.AttackPower != 0)
                fields.Add(new LevelFloat(statsLvl1.AttackPower, statsLvl10.AttackPower).UseKey(LevelFieldKeys.ATTACK_POWER));
            
            if(statsLvl1.AttackDamage != 0 || statsLvl10.AttackDamage != 0)
                fields.Add(new LevelFloat(statsLvl1.AttackDamage, statsLvl10.AttackDamage).UseKey(LevelFieldKeys.ATTACK_DAMAGE));
            
            if(statsLvl1.Armor != 0 || statsLvl10.Armor != 0)
                fields.Add(new LevelFloat(statsLvl1.Armor, statsLvl10.Armor).UseKey(LevelFieldKeys.ARMOR));
            
            if(statsLvl1.AbilityDamage != 0 || statsLvl10.AbilityDamage != 0)
                fields.Add(new LevelFloat(statsLvl1.AbilityDamage, statsLvl10.AbilityDamage).UseKey(LevelFieldKeys.ABILITY_DAMAGE).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.Mutagenicity != 0 || statsLvl10.Mutagenicity != 0)
                fields.Add(new LevelFloat(statsLvl1.Mutagenicity, statsLvl10.Mutagenicity).UseKey(LevelFieldKeys.MUTAGENICITY).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.BonusSummonAmount != 0 || statsLvl10.BonusSummonAmount != 0)
                fields.Add(new LevelFloat(statsLvl1.BonusSummonAmount, statsLvl10.BonusSummonAmount).UseKey(LevelFieldKeys.BONUS_SUMMON_AMOUNT));
            
            if(statsLvl1.SummonDamage != 0 || statsLvl10.SummonDamage != 0)
                fields.Add(new LevelFloat(statsLvl1.SummonDamage, statsLvl10.SummonDamage).UseKey(LevelFieldKeys.SUMMON_DAMAGE).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.CooldownReduction != 0 || statsLvl10.CooldownReduction != 0)
                fields.Add(new LevelFloat(statsLvl1.CooldownReduction, statsLvl10.CooldownReduction).UseKey(LevelFieldKeys.COOLDOWN_REDUCTION).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.PassiveProcRate != 0 || statsLvl10.PassiveProcRate != 0)
                fields.Add(new LevelFloat(statsLvl1.PassiveProcRate, statsLvl10.PassiveProcRate).UseKey(LevelFieldKeys.BONUS_PROC_CHANCE).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.ProjectileAmount != 0 || statsLvl10.ProjectileAmount != 0)
                fields.Add(new LevelFloat(statsLvl1.ProjectileAmount, statsLvl10.ProjectileAmount).UseKey(LevelFieldKeys.BONUS_PARTICLES_AMOUNT).UseFormatter(StatFormatter.PERCENT));
            
            if(statsLvl1.ProjectileSize != 0 || statsLvl10.ProjectileSize != 0)
                fields.Add(new LevelFloat(statsLvl1.ProjectileSize, statsLvl10.ProjectileSize).UseKey(LevelFieldKeys.BONUS_PARTICLES_SIZE).UseFormatter(StatFormatter.PERCENT));

            return fields.ToArray();
        }
    }
}