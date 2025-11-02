using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Player;
using Hitboxes;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Mutations.Active
{
    public class Dig : ActiveAbility
    {
        [SerializeField] private Transform spriteTransform;
        
        [SerializeField, Range(1, 10)] private float durationLvl1; 
        [SerializeField, Range(1, 10)] private float durationLvl10;

        private float duration;
        private int VoteSource => GetHashCode(); 


        

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            duration = LerpLevel(durationLvl1, durationLvl10, lvl);
        }

        public override void Activate()
        {
            ActivateTask(
                CancellationTokenSource.CreateLinkedTokenSource(
                    gameObject.GetCancellationTokenOnDestroy(), 
                    MainMenu.CancellationTokenOnReset).Token
                ).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            PlayerMovement.CancelKnockback();
            AttackController.CancelAttack();
            PlayerHitbox.Immune.Vote(VoteSource);
            spriteTransform.gameObject.SetActive(true);
            PlayerMovement.CanMove = false;
            PlayerAnimator.LockState(PlayerAnimatorState.None);
            PlayerPhysicsBody.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            PlayerPhysicsBody.PhysicsCollider.enabled = false;
            PlayerLocatorBody.Enabled = false;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            PlayerHitbox.Immune.Unvote(VoteSource);

            await UniTask.Delay(TimeSpan.FromSeconds(duration - 0.5f), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            spriteTransform.gameObject.SetActive(false);
            PlayerMovement.CanMove = true;
            PlayerAnimator.UnlockState();
            PlayerPhysicsBody.ResetConstraints();
            PlayerPhysicsBody.PhysicsCollider.enabled = true;
            PlayerLocatorBody.Enabled = true;
        }

        public override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}