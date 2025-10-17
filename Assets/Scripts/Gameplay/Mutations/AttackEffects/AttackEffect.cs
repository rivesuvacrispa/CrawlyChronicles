using System;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.AttackEffects
{
    public class AttackEffect
    {
        public Gradient Color { get; }
        public bool Guaranteed { get; }
        private readonly Action<IImpactable, float> onImpact;
        
        public AttackEffect(Gradient color, Action<IImpactable, float> onImpact, bool guaranteed)
        {
            Guaranteed = guaranteed;
            Color = color;
            this.onImpact = onImpact;
        }

        public void Impact(IImpactable impactable, float damage) => onImpact(impactable, damage);
    }
}