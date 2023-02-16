using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerStats
    {
        [SerializeField] private bool godmode;
        [SerializeField, Range(0.5f, 2f)] private float immunityDuration;
        [SerializeField] private int maxHealth;
        [SerializeField] private float attackKnockback;
        [SerializeField] private int attackDamage;

        public bool Godmode => godmode;
        public int MaxHealth => maxHealth;
        public float AttackKnockback => attackKnockback;
        public int AttackDamage => attackDamage;
        public float ImmunityDuration => immunityDuration;
    }
}