using System;
using Cysharp.Threading.Tasks;
using Hitboxes;
using UnityEngine;
using Util.Interfaces;

namespace Gameplay.Player
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] private PlayerAttack playerAttack;
        [Header("Stats")]
        [SerializeField] private float comboExpirationTime = 1f;

        private int comboCounter;

        public delegate void ComboEvent(int combo);

        public static event ComboEvent OnCombo;
        public static event ComboEvent OnComboExpired;
        public static event ComboEvent OnComboReady;
        public static event ComboEvent OnComboBroken;

        private void OnEnable()
        {
            IDamageable.OnDamageTakenGlobal += OnGlobalDamageTaken;
        }

        private void OnGlobalDamageTaken(IDamageable damageable, DamageInstance instance)
        {
            if (damageable is IDamageableEnemy && instance.source.owner.Equals(playerAttack));
        }


        public void ExpireCombo()
        {
            comboCounter = 0;
        }

        private async UniTask ComboExpirationTask()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(comboExpirationTime));
            ExpireCombo();
        }
    }
}