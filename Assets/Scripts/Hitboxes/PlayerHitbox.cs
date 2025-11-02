using System;
using System.Collections;
using Gameplay.Player;
using UnityEngine;
using Util;
using Util.Interfaces;

namespace Hitboxes
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHitbox : MonoBehaviour, IDamageableHitbox
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private Gradient immunityGradient;

        private int ImmuneSourceFromDamage => HashCode.Combine(GetHashCode(), 0);
        private int ImmuneSourceFromEnabled => HashCode.Combine(GetHashCode(), 1);
        
        public static readonly MultiSourceState Immune = new();

        public void Hit(DamageInstance instance) => StartCoroutine(ImmunityRoutine());

        public void Die() { }

        public bool ImmuneToSource(DamageSource source) => Immune.State;

        private IEnumerator ImmunityRoutine()
        {
            Immune.Vote(ImmuneSourceFromDamage);
            float duration = PlayerManager.PlayerStats.ImmunityDuration;
            bodyPainter.Paint(immunityGradient, duration);
            yield return new WaitForSeconds(duration);
            Immune.Unvote(ImmuneSourceFromDamage);
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            Immune.Unvote(ImmuneSourceFromEnabled);
        }

        public void Disable()
        {
            StopAllCoroutines();
            Immune.Vote(ImmuneSourceFromEnabled);
        }
    }
}