using Scriptable;
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

        private static int[] stateToAnimation = {
            IdleHash,
            WalkHash,
            DeadHash,
            NoneHash
        };
        
        

        private PlayerAnimator() => instance = this;

        private void OnEnable()
        {
            CharacterManager.OnCharacterSelected += OnCharacterSelected;
        }

        private void OnDisable()
        {
            CharacterManager.OnCharacterSelected -= OnCharacterSelected;
        }

        private void OnCharacterSelected(Character selected)
        {
            stateToAnimation = new[]
            {
                selected.IdleAnimHash,
                selected.WalkAnimHash,
                selected.DeadAnimHash,
                NoneHash
            };
        }

        public static void PlayIdle() => TryPlayAnimation(PlayerAnimatorState.Idle);
        public static void PlayWalk() => TryPlayAnimation(PlayerAnimatorState.Walk);
        public static void PlayDead() => TryPlayAnimation(PlayerAnimatorState.Dead);
        public static void PlayBuried() => TryPlayAnimation(PlayerAnimatorState.None);

        public static void LockState(PlayerAnimatorState state)
        {
            isLocked = true;
            lockedState = state;
            instance.animator.Play(AnimFromState(lockedState));
        }

        private static int AnimFromState(PlayerAnimatorState state) => stateToAnimation[(int)state];
        private static void TryPlayAnimation(PlayerAnimatorState state) 
            => instance.animator.Play(isLocked ? AnimFromState(lockedState) : AnimFromState(state));

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