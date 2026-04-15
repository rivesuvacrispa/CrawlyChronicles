using Gameplay.Effects.ChainLightning;
using Gameplay.Player;
using Hitboxes;
using Pooling;
using UnityEngine;
using Util.Abilities;
using Util.Attributes;

namespace Gameplay.Mutations.Active
{
    public class StaticChargers : ActiveAbility
    {
        [SerializeField, MinMaxRange(0, 10)] private LevelFloat damage = new LevelFloat(1, 3); 
        [SerializeField, MinMaxRange(0, 5)] private LevelFloat chainRange = new LevelFloat(1.25f, 3.5f); 
        [SerializeField, MinMaxRange(1, 10)] private LevelInt maxJumps = new LevelInt(1, 5); 
        [SerializeField, MinMaxRange(0, 2)] private LevelFloat stunDuration = new LevelFloat(0.25f, 1.25f); 
        [SerializeField, MinMaxRange(0, 1)] private LevelFloat jumpDamageReduction = new LevelFloat(0.5f, 0.8f); 

        private float currentDamage;
        private float currentChainRange;
        private int currentNumberOfJumps;
        private float currentStunDuration;
        private float currentDmgReduction;


        
        protected override ILevelField[] CreateLevelFields(int lvl)
        {
            return new[]
            {
                Scriptable.Cooldown,
                damage.UseKey(LevelFieldKeys.DAMAGE),
                chainRange.UseKey(LevelFieldKeys.EFFECT_RANGE),
                maxJumps.UseKey(LevelFieldKeys.JUMPS_AMOUNT),
                stunDuration.UseKey(LevelFieldKeys.STUN_DURATION).UseFormatter(StatFormatter.SECONDS),
                jumpDamageReduction.UseKey(LevelFieldKeys.JUMP_DAMAGE_REDUCTION)
            };
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            currentDamage = damage.AtLvl(lvl);
            currentChainRange = chainRange.AtLvl(lvl);
            currentNumberOfJumps = maxJumps.AtLvl(lvl);
            currentStunDuration = stunDuration.AtLvl(lvl);
            currentDmgReduction = jumpDamageReduction.AtLvl(lvl);
        }

        public override void Activate(bool auto = false)
        {
            Vector3 pos = PlayerPhysicsBody.Position;
            if (ChainLightning.TryGetTarget(pos, currentChainRange, out IDamageableEnemy _))
            {
                base.Activate(auto);
                PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                    currentDamage, currentChainRange, currentNumberOfJumps, null, 0, pos, currentStunDuration, currentDmgReduction));
            }
            else
            {
                SetOnCooldown(1f);
            }
        }
    }
}