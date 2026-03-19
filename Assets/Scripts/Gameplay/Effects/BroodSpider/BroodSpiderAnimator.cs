using UnityEngine;
using Util;

namespace Gameplay.Effects.BroodSpider
{
    public class BroodSpiderAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer shadowRenderer;
        [SerializeField] private Transform bodySpriteTransform;

        private static readonly int WalkHash = Animator.StringToHash("BroodSpiderWalk");
        private static readonly int IdleHash = Animator.StringToHash("BroodSpiderIdle");
        private static readonly int DeadHash = Animator.StringToHash("BroodSpiderDead");

        public void ResetRotation() => bodySpriteTransform.rotation = Quaternion.identity;
        
        public void RotateTowards(Vector3 pos)
        {
            bodySpriteTransform.RotateTowardsPosition(pos, 360);
        }
        
        public void PlayWalk()
        {
            Debug.Log($"{gameObject.name}: Play walk");
            animator.Play(WalkHash);
        }

        public void PlayIdle()
        {
            Debug.Log($"{gameObject.name}: Play idle");
            animator.Play(IdleHash);
        }

        public void PlayDead()
        {
            Debug.Log($"{gameObject.name}: Play dead");
            animator.Play(DeadHash);
        }

        public void PlayAttack()
        {
            Debug.Log($"{gameObject.name}: Play attack");
            PlayIdle();
            shadowRenderer.enabled = true;
            bodySpriteTransform.localPosition = new Vector3(0, 0.2f, 0f);
        }

        public void StopAttack()
        {
            shadowRenderer.enabled = false;
            bodySpriteTransform.localPosition = Vector3.zero;
        }
    }
}