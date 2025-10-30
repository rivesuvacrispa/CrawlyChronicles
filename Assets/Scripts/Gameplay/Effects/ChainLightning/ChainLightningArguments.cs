using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Effects.ChainLightning
{
    public readonly struct ChainLightningArguments
    {
        public readonly float damage;
        public readonly float chainRange;
        public readonly float maxNumberOfJumps;
        public readonly IDamageable currentTarget;
        public readonly int currentJump;
        public readonly Vector3 position;
        public readonly float stunDuration;
        public readonly float dmgReduction;

        public ChainLightningArguments(float damage, float chainRange, float maxNumberOfJumps, IDamageable currentTarget, int currentJump, Vector3 position, float stunDuration, float dmgReduction)
        {
            this.damage = damage;
            this.chainRange = chainRange;
            this.maxNumberOfJumps = maxNumberOfJumps;
            this.currentTarget = currentTarget;
            this.currentJump = currentJump;
            this.position = position;
            this.stunDuration = stunDuration;
            this.dmgReduction = dmgReduction;
        }
    }
}