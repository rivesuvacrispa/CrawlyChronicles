using UnityEngine;

namespace Gameplay.Bosses.BlackWidow
{
    public class BlackWidowAnimatorBody : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private readonly int WalkAnimHash = Animator.StringToHash("Black Widow Walk");

        public void SetSpeed(float speed) => animator.speed = speed;
        
        private void OnEnable()
        {
            animator.Play(WalkAnimHash);
        }
    }
}