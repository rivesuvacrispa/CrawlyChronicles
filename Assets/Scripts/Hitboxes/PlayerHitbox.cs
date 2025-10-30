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

        private bool immune;

        public void Hit(DamageInstance instance) => StartCoroutine(ImmunityRoutine());

        public void Die() { }

        public bool ImmuneToSource(DamageSource source) => immune;

        private IEnumerator ImmunityRoutine()
        {
            immune = true;
            float duration = PlayerManager.PlayerStats.ImmunityDuration;
            bodyPainter.Paint(immunityGradient, duration);
            yield return new WaitForSeconds(duration);
            immune = false;
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            immune = false;
        }

        public void Disable()
        {
            StopAllCoroutines();
            immune = true;
        }
    }
}