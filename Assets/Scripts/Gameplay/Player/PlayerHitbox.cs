using System.Collections;
using UnityEngine;
using Util;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerHitbox : MonoBehaviour
    {
        [SerializeField] private BodyPainter bodyPainter;
        [SerializeField] private Gradient immunityGradient;
        
        public bool Immune { get; private set; }
        
        public bool BlockColorChange { get; set; }

        

        public void Hit() => StartCoroutine(ImmunityRoutine());

        private IEnumerator ImmunityRoutine()
        {
            Immune = true;

            float duration = PlayerManager.PlayerStats.ImmunityDuration;
            bodyPainter.Paint(immunityGradient, duration);
            yield return new WaitForSeconds(duration);
            
            Immune = false;
        }
        
        public void Enable()
        {
            StopAllCoroutines();
            Immune = false;
        }

        public void Disable()
        {
            StopAllCoroutines();
            Immune = true;
        }
    }
}