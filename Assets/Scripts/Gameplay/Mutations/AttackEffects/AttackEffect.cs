using System;
using Gameplay.Enemies;
using UnityEngine;

namespace Mutations.AttackEffects
{
    public class AttackEffect
    {
        public Gradient Color { get; }
        private readonly Action<Enemy, float> onImpact;
        
        public AttackEffect(Gradient color, Action<Enemy, float> onImpact)
        {
            Color = color;
            this.onImpact = onImpact;
        }

        public void Impact(Enemy enemy, float damage) => onImpact(enemy, damage);
    }
}