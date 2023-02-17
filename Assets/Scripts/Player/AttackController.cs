using System.Collections;
using Camera;
using Gameplay.Interaction;
using UnityEngine;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private Movement movementComponent;
        [SerializeField] private Animator animator;
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float comboRotationSpeed;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private float comboExpirationTime;

        private int comboCounter;
        private Coroutine comboExpirationRoutine;

        public bool IsAttacking => attackGO.activeInHierarchy;

        private void Update()
        {
            if(Interactor.Interacting) return;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (comboCounter == 3)
                    ComboAttack();
                else
                    Attack();
            } 
        }

        private void Attack()
        {
            if (movementComponent.Dash(MainCamera.WorldMousePos, dashDuration, dashSpeed, 
                () => {
                    attackGO.SetActive(false);
                    StartComboExpiration();
                    hitbox.Enable();
                }))
            {
                if(comboExpirationRoutine is not null) 
                    StopCoroutine(comboExpirationRoutine);
                comboCounter++;
                attackGO.SetActive(true);
                // animator.Play(claw1Hash);
                hitbox.Disable();
            }
        }

        private void ComboAttack()
        {
            if (movementComponent.ComboDash(dashDuration * 2, comboRotationSpeed, 
                () => {
                    attackGO.SetActive(false);
                    ExpireCombo();
                    hitbox.Enable();
                }))
            {
                StopCoroutine(comboExpirationRoutine);
                attackGO.SetActive(true);
                hitbox.Disable();
            }
        }

        private void StartComboExpiration()
        {
            comboExpirationRoutine = StartCoroutine(ComboExpirationRoutine());
        }

        private void ExpireCombo()
        {
            comboCounter = 0;
            comboExpirationRoutine = null;
            // animator.Play(claw0Hash);
        }

        private IEnumerator ComboExpirationRoutine()
        {
            yield return new WaitForSeconds(comboExpirationTime);
            ExpireCombo();
        }
    }
}