using System.Text;
using UnityEngine;
using Util;

namespace Player
{
    [System.Serializable]
    public struct PlayerStats
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float maxHealth;
        [SerializeField] private float attackPower;
        [SerializeField] private float attackDamage;
        [SerializeField] private float armor;
        [SerializeField] private float immunityDuration;
        [SerializeField] private float abilityDamage;
        [SerializeField] private float passiveProcRate;

        public float MaxHealth => maxHealth;
        public float AttackPower => attackPower;
        public float AttackDamage => attackDamage;
        public float Armor => armor;
        public float ImmunityDuration => immunityDuration;
        public float MovementSpeed => movementSpeed;
        public float RotationSpeed => rotationSpeed;
        public float AbilityDamage => abilityDamage;
        public float PassiveProcRate => passiveProcRate;


        public PlayerStats(float movementSpeed = 0, float rotationSpeed = 0, float maxHealth = 0, 
            float attackPower = 0, float attackDamage = 0, float armor = 0, float immunityDuration = 0,
            float abilityDamage = 0, float passiveProcRate = 0)
        {
            this.movementSpeed = movementSpeed;
            this.rotationSpeed = rotationSpeed;
            this.maxHealth = maxHealth;
            this.attackPower = attackPower;
            this.attackDamage = attackDamage;
            this.armor = armor;
            this.immunityDuration = immunityDuration;
            this.abilityDamage = abilityDamage;
            this.passiveProcRate = passiveProcRate;
        }

        public void AddStats(PlayerStats baseStats, PlayerStats stats)
        {
            movementSpeed = Mathf.Clamp(MovementSpeed + stats.MovementSpeed, baseStats.MovementSpeed, float.MaxValue);
            rotationSpeed = Mathf.Clamp(RotationSpeed + stats.RotationSpeed, baseStats.RotationSpeed, float.MaxValue);
            maxHealth = Mathf.Clamp(MaxHealth + stats.MaxHealth, baseStats.MaxHealth, float.MaxValue);
            attackPower = Mathf.Clamp(AttackPower + stats.AttackPower, baseStats.AttackPower, float.MaxValue);
            attackDamage = Mathf.Clamp(AttackDamage + stats.AttackDamage, baseStats.AttackDamage, float.MaxValue);
            armor = Mathf.Clamp(Armor + stats.Armor, baseStats.Armor, float.MaxValue);
            immunityDuration = Mathf.Clamp(ImmunityDuration + stats.ImmunityDuration, baseStats.ImmunityDuration, float.MaxValue);
            abilityDamage = Mathf.Clamp(abilityDamage + stats.abilityDamage, baseStats.abilityDamage, float.MaxValue);
            passiveProcRate = Mathf.Clamp(passiveProcRate + stats.passiveProcRate, baseStats.passiveProcRate, float.MaxValue);
        }

        public static PlayerStats LerpLevel(PlayerStats lvl1, PlayerStats lvl10, int level)
        {
            return new PlayerStats(
                movementSpeed: Mathf.Lerp(lvl1.MovementSpeed,lvl10.MovementSpeed,level / 9f),
                rotationSpeed: Mathf.Lerp(lvl1.RotationSpeed,lvl10.RotationSpeed,level / 9f),
                maxHealth: Mathf.Lerp(lvl1.MaxHealth,lvl10.MaxHealth,level / 9f),
                attackPower: Mathf.Lerp(lvl1.AttackPower,lvl10.AttackPower,level / 9f),
                attackDamage: Mathf.Lerp(lvl1.AttackDamage,lvl10.AttackDamage,level / 9f),
                armor: Mathf.Lerp(lvl1.Armor,lvl10.Armor,level / 9f),
                immunityDuration: Mathf.Lerp(lvl1.ImmunityDuration,lvl10.ImmunityDuration,level / 9f),
                abilityDamage: Mathf.Lerp(lvl1.abilityDamage,lvl10.abilityDamage,level / 9f),
                passiveProcRate: Mathf.Lerp(lvl1.passiveProcRate,lvl10.passiveProcRate,level / 9f));
        }

        public PlayerStats Negated()
        {
            return new PlayerStats(
                movementSpeed: -MovementSpeed,
                rotationSpeed: -RotationSpeed,
                maxHealth: -MaxHealth,
                attackPower: -AttackPower,
                attackDamage: -AttackDamage,
                armor: -Armor,
                immunityDuration: -ImmunityDuration,
                abilityDamage: -abilityDamage,
                passiveProcRate: -passiveProcRate);
        }

        public static PlayerStats Zero => new();

        public string Print(bool includeZeros)
        {
            StringBuilder sb = new StringBuilder();
            if(includeZeros || MovementSpeed > 0) 
                sb.AppendColored("orange","Movespeed: ").Append($"{MovementSpeed:n1}\n");
            if(includeZeros || RotationSpeed > 0) 
                sb.AppendColored("orange","Rotation speed: ").Append($"{RotationSpeed:n1}\n");
            if(includeZeros || MaxHealth > 0) 
                sb.AppendColored("orange","Max health: ").Append($"{MaxHealth:n1}\n");
            if(includeZeros || AttackPower > 0) 
                sb.AppendColored("orange","Attack power: ").Append($"{AttackPower:n1}\n");
            if(includeZeros || AttackDamage > 0) 
                sb.AppendColored("orange","Attack damage: ").Append($"{AttackDamage:n1}\n");
            if(includeZeros || Armor > 0) 
                sb.AppendColored("orange","Armor: ").Append($"{Armor:n1}\n");
            if(includeZeros || ImmunityDuration > 0) 
                sb.AppendColored("orange","Immunity frame: ").Append($"{ImmunityDuration:n1}\n");
            if(includeZeros || abilityDamage > 0) 
                sb.AppendColored("orange","Ability damage: ").Append($"{(int) (abilityDamage * 100)}%\n");
            return sb.ToString();
        }

        public string PrintCompared(PlayerStats with)
        {
            StringBuilder sb = new StringBuilder();

            if (MovementSpeed > 0) sb.AddAbilityLine("Movespeed", MovementSpeed, with.movementSpeed);
            if (RotationSpeed > 0) sb.AddAbilityLine("Rotation speed", RotationSpeed, with.rotationSpeed);
            if (MaxHealth > 0) sb.AddAbilityLine("Max health", MaxHealth, with.maxHealth);
            if (AttackPower > 0) sb.AddAbilityLine("Attack power", AttackPower, with.attackPower);
            if (AttackDamage > 0) sb.AddAbilityLine("Attack damage", AttackDamage, with.attackDamage);
            if (Armor > 0) sb.AddAbilityLine("Armor", Armor, with.armor);
            if (ImmunityDuration > 0) sb.AddAbilityLine("Immunity frame", ImmunityDuration, with.immunityDuration, suffix: "s");
            if (abilityDamage > 0) sb.AddAbilityLine("Ability damage", abilityDamage, with.abilityDamage, percent: true, prefix: "+", suffix: "%");

            return sb.ToString();
        }
    }
}