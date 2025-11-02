using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        private static PlayerAnimator instance;
        
        [SerializeField] private Animator animator;
        
        private static readonly int IdleHash = Animator.StringToHash("PlayerSpriteIdle");
        private static readonly int WalkHash = Animator.StringToHash("PlayerSpriteWalk");
        private static readonly int DeadHash = Animator.StringToHash("PlayerSpriteDead");
        private static readonly int NoneHash = Animator.StringToHash("PlayerSpriteNone");

        private static PlayerAnimatorState lockedState;
        private static bool isLocked;

        private static readonly int[] StateToAnimation = {
            IdleHash,
            WalkHash,
            DeadHash,
            NoneHash
        };
        
        

        private PlayerAnimator() => instance = this;

        public static void PlayIdle() => instance.animator.Play(isLocked ? StateToAnimation[(int)lockedState] : IdleHash);
        public static void PlayWalk() => instance.animator.Play(isLocked ? StateToAnimation[(int)lockedState] : WalkHash);
        public static void PlayDead() => instance.animator.Play(isLocked ? StateToAnimation[(int)lockedState] : DeadHash);
        public static void PlayBuried() => instance.animator.Play(isLocked ? StateToAnimation[(int)lockedState] : NoneHash);

        public static void LockState(PlayerAnimatorState state)
        {
            isLocked = true;
            lockedState = state;
            instance.animator.Play(StateToAnimation[(int)lockedState]);
        }

        public static void UnlockState()
        {
            isLocked = false;
            PlayIdle();
        }
    }

    public enum PlayerAnimatorState
    {
        Idle = 0,
        Walk = 1,
        Dead = 2,
        None = 3
    }
}