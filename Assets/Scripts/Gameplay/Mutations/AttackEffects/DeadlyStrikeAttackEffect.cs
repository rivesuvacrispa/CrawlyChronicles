using System;
using Definitions;
using Gameplay.Effects.DamageText;
using Gameplay.Player;
using TMPro;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Mutations.AttackEffects
{
    public class DeadlyStrikeAttackEffect : AttackEffect, ICanChangeDamageText
    {
        private BasePlayerAttack attack;

        private static readonly DamageTextProperties TextProperties = new(
            new VertexGradient(UnityEngine.Color.yellow, UnityEngine.Color.yellow, UnityEngine.Color.red, UnityEngine.Color.red), 
            GlobalDefinitions.BloodyFont, 3f, 2.25f
        );


        public DeadlyStrikeAttackEffect(
            Gradient color,
            Action<IImpactable, float> onImpact,
            float bonusDamage)
            : base(color, onImpact)
        {
            this.onApplied += a =>
            {
                attack = a;
                attack.AddBonusDamage(bonusDamage);
            };
            this.onImpact += (_, _) =>
            {
                attack.AddBonusDamage(-bonusDamage);
                attack.RemoveEffect(this);
            };
        }

        // ICanChangeDamageText
        public bool ShouldChangeDamageText() => true;

        public DamageTextProperties GetDamageTextProperties() => TextProperties;
    }
}