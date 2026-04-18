using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerBodyAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private static readonly int BreedAnimHash = Animator.StringToHash("PlayerBodyBreeding");
        private static readonly int IdleAnimHash = Animator.StringToHash("PlayerBodyIdle");

        private static Animator Animator { get; set; }

        private void Awake()
        {
            Animator = animator;
        }
        
        public static void PlayBreed()
        {
            Animator.Play(BreedAnimHash);
        }

        public static void PlayIdle()
        {
            Animator.Play(IdleAnimHash);
        }
    }
}