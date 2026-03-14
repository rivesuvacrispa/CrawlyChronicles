using System;
using Gameplay.Player;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.AttackEffects
{
    public class AttackEffect
    {
        public Gradient Color { get; }
        protected Action<IImpactable, float> onImpact;
        protected Action<BasePlayerAttack> onApplied;
        
        public AttackEffect(Gradient color, Action<IImpactable, float> onImpact, Action<BasePlayerAttack> onApplied = null)
        {
            Color = color;
            this.onImpact = onImpact;
            this.onApplied = onApplied ?? (_ => { });
        }

        public void Impact(IImpactable impactable, float damage) => onImpact(impactable, damage);

        public void Apply(BasePlayerAttack attack) => onApplied(attack);
    }
}