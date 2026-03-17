using Gameplay.Effects.ChainLightning;
using Gameplay.Player;
using Hitboxes;
using Pooling;
using UnityEngine;

namespace Gameplay.Mutations.Passive
{
    public class StaticChargers : ActiveAbility
    {
        [Header("Damage")]
        [SerializeField] private float damageLvl1;
        [SerializeField] private float damageLvl10;        
        [Header("Chain Range")]
        [SerializeField] private float chainRangeLvl1;
        [SerializeField] private float chainRangeLvl10;
        [Header("Duration")] 
        [SerializeField, Range(1, 10)] private int maxNumberOfJumpsLvl1;
        [SerializeField, Range(1, 10)] private int maxNumberOfJumpsLvl10;
        [Header("Stun Duration")]
        [SerializeField, Range(0, 2)] private float stunDurationLvl1;
        [SerializeField, Range(0, 2)] private float stunDurationLvl10;
        [Header("Damage Reduction Over Jump")]
        [SerializeField, Range(0, 1)] private float dmgReductionLvl1;
        [SerializeField, Range(0, 1)] private float dmgReductionLvl10;
        
        
        private float damage;
        private float chainRange;
        private int maxNumberOfJumps;
        private float stunDuration;
        private float dmgReduction;


        
        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            damage = LerpLevel(damageLvl1, damageLvl10, lvl);
            chainRange = LerpLevel(chainRangeLvl1, chainRangeLvl10, lvl);
            maxNumberOfJumps = Mathf.RoundToInt(LerpLevel(maxNumberOfJumpsLvl1, maxNumberOfJumpsLvl10, lvl));
            stunDuration = LerpLevel(stunDurationLvl1, stunDurationLvl10, lvl);
            dmgReduction = LerpLevel(dmgReductionLvl1, dmgReductionLvl10, lvl);
        }

        public override void Activate(bool auto = false)
        {
            Vector3 pos = PlayerPhysicsBody.Position;
            if (ChainLightning.TryGetTarget(pos, chainRange, out IDamageableEnemy _))
            {
                base.Activate(auto);
                PoolManager.GetEffect<ChainLightning>(new ChainLightningArguments(
                    damage, chainRange, maxNumberOfJumps, null, 0, pos, stunDuration, dmgReduction));
            }
            else
            {
                SetOnCooldown(1f);
            }
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}