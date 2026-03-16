using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Hitboxes;
using SoundEffects;
using UnityEngine;

namespace Gameplay.Player
{
    public class AttackController : MonoBehaviour
    {
        public static AttackController Instance { get; private set; }
        
        [Header("Refs")]
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private BasePlayerAttack attack;
        [SerializeField] private Vector3 defaultAttackPosition;
        [SerializeField] private Vector3 comboAttackPosition;

        [Header("Stats")]
        [SerializeField] private float dashDuration;

        private Coroutine comboExpirationRoutine;

        public delegate void AttackControllerEvent();
        public static event AttackControllerEvent OnAttackStart;
        private static CancellationTokenSource cancellationTokenSource;

        public bool IsAttacking => attack.IsActive;
        public static bool IsInComboDash { get; private set; }



        private AttackController() => Instance = this;
        
        private void Awake()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public static void CancelAttack()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void Update()
        {
            if (Interactor.Interacting || 
                !Input.GetMouseButtonDown(0) ||
                Time.timeScale == 0 ||
                !PlayerMovement.Enabled ||
                !PlayerMovement.CanMove)
                return;
            
            Attack(PlayerManager.PlayerStats.AttackPower * PlayerMovement.MoveSpeedAmplifier,
                cancellationTokenSource.Token).Forget();
        }
        
        public async UniTask Attack(float force, CancellationToken cancellationToken)
        {
            OnAttackStart?.Invoke();
            if(comboExpirationRoutine is not null) 
                StopCoroutine(comboExpirationRoutine);
            attack.Enable();
            hitbox.Disable();

            await PlayerMovement.Dash(
                    dashDuration, 
                    force,
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            attack.Disable();
            // StartComboExpiration();
            hitbox.Enable();
        }

        public async UniTask WhirlwindAttack(float duration, CancellationToken cancellationToken)
        {
            OnAttackStart?.Invoke();
            IsInComboDash = true;
            PlayerAudioController.Instance.PlayCombo();
            if(comboExpirationRoutine is not null) 
                StopCoroutine(comboExpirationRoutine);
            attack.transform.localPosition = comboAttackPosition;
            attack.Enable();
            hitbox.Disable();

            await PlayerMovement.ComboDash(duration, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            attack.transform.localPosition = defaultAttackPosition;
            attack.Disable();
            hitbox.Enable();
            PlayerAudioController.Instance.StopAction();
            IsInComboDash = false;
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
    }
}