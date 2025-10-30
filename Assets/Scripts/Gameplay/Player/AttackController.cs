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
        [Header("Refs")]
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private PlayerMovement movementComponent;
        [SerializeField] private ComboManager comboManager;
        [SerializeField] private PlayerAttack attack;
        [SerializeField] private Vector3 defaultAttackPosition;
        [SerializeField] private Vector3 comboAttackPosition;

        [Header("Stats")]
        [SerializeField] private float dashDuration;
        [SerializeField] private float comboRotationSpeed;

        private Coroutine comboExpirationRoutine;

        public delegate void AttackControllerEvent();
        public static event AttackControllerEvent OnAttackStart;
        
        
        
        public bool IsAttacking => attack.IsActive;
        public static bool IsInComboDash { get; private set; }

        private void Update()
        {
            if (Interactor.Interacting || 
                !Input.GetMouseButtonDown(0) ||
                Time.timeScale == 0 ||
                movementComponent.InAttackDash)
                return;
            
            // if (comboCounter == 3)
                // ComboAttack(gameObject.GetCancellationTokenOnDestroy()).Forget();
            // else
            Attack(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private async UniTask Attack(CancellationToken cancellationToken)
        {
            OnAttackStart?.Invoke();
            if(comboExpirationRoutine is not null) 
                StopCoroutine(comboExpirationRoutine);
            PlayerAudioController.Instance.PlayAttack(0);
            attack.Enable();
            hitbox.Disable();

            await movementComponent.Dash(dashDuration, cancellationToken: cancellationToken);
            
            attack.Disable();
            // StartComboExpiration();
            hitbox.Enable();
        }

        private async UniTask ComboAttack(CancellationToken cancellationToken)
        {
            OnAttackStart?.Invoke();
            IsInComboDash = true;
            PlayerAudioController.Instance.PlayCombo();
            if(comboExpirationRoutine is not null) 
                StopCoroutine(comboExpirationRoutine);
            attack.transform.localPosition = comboAttackPosition;
            attack.Enable();
            hitbox.Disable();

            await movementComponent.ComboDash(dashDuration * 2, comboRotationSpeed, cancellationToken: cancellationToken);
            
            attack.transform.localPosition = defaultAttackPosition;
            attack.Disable();
            // ExpireCombo();
            hitbox.Enable();
            PlayerAudioController.Instance.StopAction();
            IsInComboDash = false;
        }
    }
}