using System.Collections;
using Gameplay.Interaction;
using Scripts.SoundEffects;
using UnityEngine;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private PlayerHitbox hitbox;
        [SerializeField] private Movement movementComponent;
        [SerializeField] private float dashDuration;
        [SerializeField] private float comboRotationSpeed;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private float comboExpirationTime;

        private int comboCounter;
        private Coroutine comboExpirationRoutine;

        public bool IsAttacking => attackGO.activeInHierarchy;
        public static bool IsInComboDash { get; private set; }

        private void Update()
        {
            if (Interactor.Interacting)
                return;
            if (Input.GetMouseButtonDown(0) && Time.timeScale != 0)
            {
                if (comboCounter == 3)
                    ComboAttack();
                else
                    Attack();
            } 
        }

        private void Attack()
        {
            if (movementComponent.Dash(dashDuration, 
                () => {
                    attackGO.SetActive(false);
                    StartComboExpiration();
                    hitbox.Enable();
                }))
            {
                if(comboExpirationRoutine is not null) 
                    StopCoroutine(comboExpirationRoutine);
                PlayerAudioController.Instance.PlayAttack(comboCounter);
                comboCounter++;
                attackGO.SetActive(true);
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
                    PlayerAudioController.Instance.StopAction();
                    IsInComboDash = false;
                }))
            {
                IsInComboDash = true;
                PlayerAudioController.Instance.PlayCombo();
                StopCoroutine(comboExpirationRoutine);
                attackGO.SetActive(true);
                hitbox.Disable();
            }
        }

        private void StartComboExpiration()
        {
            comboExpirationRoutine = StartCoroutine(ComboExpirationRoutine());
        }

        public void ExpireCombo()
        {
            comboCounter = 0;
            comboExpirationRoutine = null;
        }

        private IEnumerator ComboExpirationRoutine()
        {
            yield return new WaitForSeconds(comboExpirationTime);
            ExpireCombo();
        }
    }
}