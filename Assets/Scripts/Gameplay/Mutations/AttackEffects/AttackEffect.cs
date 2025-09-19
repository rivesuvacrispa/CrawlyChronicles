using System;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.AttackEffects
{
    public class AttackEffect
    {
        public Gradient Color { get; }
        private readonly Action<IImpactable, float> onImpact;
        
        public AttackEffect(Gradient color, Action<IImpactable, float> onImpact)
        {
            Color = color;
            this.onImpact = onImpact;
        }

        public void Impact(IImpactable impactable, float damage) => onImpact(impactable, damage);
    }
}