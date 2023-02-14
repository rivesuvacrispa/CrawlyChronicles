using System.Collections;
using Camera;
using UnityEngine;

namespace Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private Movement movementComponent;
        [SerializeField] private Animator animator;
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float comboRotationSpeed;
        [SerializeField] private GameObject attackGO;
        [SerializeField] private float comboExpirationTime;
        [SerializeField] private float knockbackPower;
        [SerializeField] private int attackDamage;

        private readonly int claw0Hash = Animator.StringToHash("Claw0");
        private readonly int claw1Hash = Animator.StringToHash("Claw1");
        private int comboCounter;
        private Coroutine comboExpirationRoutine;

        public static float KnockbackPower { get; private set; }
        public static int AttackDamage { get; private set; }

        private void Start()
        {
            KnockbackPower = knockbackPower;
            AttackDamage = attackDamage;
        }

        private void Update()
        {
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
                }))
            {
                comboCounter++;
                attackGO.SetActive(true);
                animator.Play(claw1Hash);
            }
        }

        private void ComboAttack()
        {
            if (movementComponent.ComboDash(dashDuration * 2, comboRotationSpeed, () =>
                {
                    attackGO.SetActive(false);
                }))
            {
                StopCoroutine(comboExpirationRoutine);
                ExpireCombo();
                attackGO.SetActive(true);
            }
        }

        private void StartComboExpiration()
        {
            if(comboExpirationRoutine is not null) StopCoroutine(comboExpirationRoutine);
            comboExpirationRoutine = StartCoroutine(ComboExpirationRoutine());
        }

        private void ExpireCombo()
        {
            comboCounter = 0;
            comboExpirationRoutine = null;
            animator.Play(claw0Hash);
        }

        private IEnumerator ComboExpirationRoutine()
        {
            yield return new WaitForSeconds(comboExpirationTime);
            ExpireCombo();
        }
    }
}