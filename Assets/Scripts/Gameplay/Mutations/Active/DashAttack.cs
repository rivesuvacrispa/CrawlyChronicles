using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Player;
using Hitboxes;
using UnityEngine;
using Util;

namespace Gameplay.Mutations.Active
{
    public class DashAttack : ActiveAbility
    {
        [Header("References")] 

        [SerializeField] private DashPlayerAttack attack;
        [Header("Force")]
        [SerializeField, Range(0, 10f)] private float forceLvl1;
        [SerializeField, Range(0, 10f)] private float forceLvl10;
        [Header("Size")]
        [SerializeField, Range(0.1f, 10f)] private float sizeLvl1;
        [SerializeField, Range(0.1f, 10f)] private float sizeLvl10;
        
        private float force;
        private int VoteSource => GetHashCode();
        private float size;


        
        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerSizeManager.OnSizeChanged += OnPlayerSizeChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerSizeManager.OnSizeChanged -= OnPlayerSizeChanged;
        }

        private void OnPlayerSizeChanged(float f)
        {
            size = LerpLevel(sizeLvl1, sizeLvl10, level) * PlayerSizeManager.CurrentSize;
            attack.UpdateSize(size);
        }

        public override void OnLevelChanged(int lvl)
        {
            base.OnLevelChanged(lvl);
            force = LerpLevel(forceLvl1, forceLvl10, lvl);
            size = LerpLevel(sizeLvl1, sizeLvl10, lvl) * PlayerSizeManager.CurrentSize;
            attack.UpdateSize(size);
        }

        public override void Activate(bool auto = false)
        {
            base.Activate(auto);
            
            Vector2 mousePos = MainCamera.WorldMousePos;
            PlayerPhysicsBody.Rigidbody.RotateTowardsPosition(mousePos, 360);
            
            ActivateTask(CreateCommonCancellationToken()).Forget();
        }

        private async UniTask ActivateTask(CancellationToken cancellationToken)
        {
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            
            AttackController.CancelAttack();
            PlayerMovement.CancelKnockback();
            
            attack.Enable();
            PlayerHitbox.Immune.Vote(VoteSource);
            PlayerMovement.CanMove = false;
            PlayerAnimator.LockState(PlayerAnimatorState.Idle);
            PlayerPhysicsBody.Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerPhysicsBody.PhysicsCollider.enabled = false;
            
            await PlayerMovement.Dash(0.4f, force, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            PlayerHitbox.Immune.Unvote(VoteSource);
            PlayerMovement.CanMove = true;
            PlayerAnimator.UnlockState();
            PlayerPhysicsBody.ResetConstraints();
            PlayerPhysicsBody.PhysicsCollider.enabled = true;
            attack.Disable();
        }

        protected override object[] GetDescriptionArguments(int lvl, bool withUpgrade)
        {
            return null;
        }
    }
}