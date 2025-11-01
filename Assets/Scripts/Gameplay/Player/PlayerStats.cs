using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Gameplay.Player
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
        [SerializeField] private float mutagenicity;
        [SerializeField] private int bonusSummonAmount;
        [SerializeField] private float summonDamage;

        public float MaxHealth => maxHealth;
        public float AttackPower => attackPower;
        public float AttackDamage => attackDamage;
        public float Armor => armor;
        public float ImmunityDuration => immunityDuration;
        public float MovementSpeed => movementSpeed;
        public float RotationSpeed => rotationSpeed;
        public float AbilityDamage => abilityDamage;
        public float PassiveProcRate => passiveProcRate;
        public float Mutagenicity => mutagenicity;
        public int BonusSummonAmount => bonusSummonAmount;
        public float SummonDamage => summonDamage;

        private static readonly TableEntryReference PlayerStatsStringReference = "Essentials_PlayerStats";
        private static readonly TableEntryReference PlayerStatsComparedStringReference = "Essentials_PlayerStatsCompared";
        private static readonly TableReference EssentialsTableReference = "UI_Essentials";


        
        
        public PlayerStats(float movementSpeed = 0, float rotationSpeed = 0, float maxHealth = 0, 
            float attackPower = 0, float attackDamage = 0, float armor = 0, float immunityDuration = 0,
            float abilityDamage = 0, float passiveProcRate = 0, float mutagenicity = 0, int bonusSummonAmount = 0,
            float summonDamage = 0)
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
            this.mutagenicity = mutagenicity;
            this.bonusSummonAmount = bonusSummonAmount;
            this.summonDamage = summonDamage;
        }

        public void AddStats(PlayerStats baseStats, PlayerStats stats)
        {
            movementSpeed = Mathf.Clamp(MovementSpeed + stats.MovementSpeed, baseStats.MovementSpeed, 100);
            rotationSpeed = Mathf.Clamp(RotationSpeed + stats.RotationSpeed, baseStats.RotationSpeed, float.MaxValue);
            maxHealth = Mathf.Clamp(MaxHealth + stats.MaxHealth, baseStats.MaxHealth, float.MaxValue);
            attackPower = Mathf.Clamp(AttackPower + stats.AttackPower, baseStats.AttackPower, float.MaxValue);
            attackDamage = Mathf.Clamp(AttackDamage + stats.AttackDamage, baseStats.AttackDamage, float.MaxValue);
            armor = Mathf.Clamp(Armor + stats.Armor, baseStats.Armor, float.MaxValue);
            immunityDuration = Mathf.Clamp(ImmunityDuration + stats.ImmunityDuration, baseStats.ImmunityDuration, 1f);
            abilityDamage = Mathf.Clamp(abilityDamage + stats.abilityDamage, baseStats.abilityDamage, float.MaxValue);
            passiveProcRate = Mathf.Clamp(passiveProcRate + stats.passiveProcRate, baseStats.passiveProcRate, 1f);
            mutagenicity = Mathf.Clamp(mutagenicity + stats.mutagenicity, baseStats.mutagenicity, 100);
            bonusSummonAmount = Mathf.Clamp(bonusSummonAmount + stats.bonusSummonAmount, baseStats.bonusSummonAmount, 100);
            summonDamage = Mathf.Clamp(summonDamage + stats.summonDamage, baseStats.summonDamage, float.MaxValue);
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
                passiveProcRate: Mathf.Lerp(lvl1.passiveProcRate,lvl10.passiveProcRate,level / 9f),
                mutagenicity: Mathf.Lerp(lvl1.mutagenicity,lvl10.mutagenicity,level / 9f),
                bonusSummonAmount: Mathf.RoundToInt(Mathf.Lerp(lvl1.bonusSummonAmount,lvl10.bonusSummonAmount,level / 9f)),
                summonDamage: Mathf.Lerp(lvl1.summonDamage,lvl10.summonDamage,level / 9f)
                );
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
                passiveProcRate: -passiveProcRate,
                mutagenicity: -mutagenicity,
                bonusSummonAmount: -bonusSummonAmount,
                summonDamage: -summonDamage
                );
        }

        public static readonly PlayerStats Zero = new();
        
        public static readonly PlayerStats Minimal = new(
            movementSpeed: 0.1f,
            rotationSpeed: 0.1f,
            maxHealth: 1,
            attackPower: 0.01f,
            attackDamage: 0.01f,
            armor: 0,
            immunityDuration: 0.05f,
            abilityDamage: 0f,
            passiveProcRate: 0,
            mutagenicity: 0f,
            bonusSummonAmount: 0,
            summonDamage: 0f
            );

        public object[] GetStringArguments()
        {
            return new object[]
            {
                MovementSpeed,
                RotationSpeed,
                MaxHealth,
                AttackPower,
                AttackDamage,
                Armor,
                (int)(abilityDamage * 100),
                (int)(mutagenicity * 100),
                bonusSummonAmount,
                (int)(summonDamage * 100)
            };
        }
        
        public string Print(object[] args = null)
        {
            return LocalizationSettings.StringDatabase
                .GetLocalizedString(EssentialsTableReference, PlayerStatsStringReference, args ?? GetStringArguments());
        }

        public string PrintCompared(PlayerStats with)
        {
            int abilityDmg = (int) (abilityDamage * 100);
            int summonDmg = (int) (summonDamage * 100);
            int muta = (int) (mutagenicity * 100);
            return LocalizationSettings.StringDatabase
                .GetLocalizedString(EssentialsTableReference, PlayerStatsComparedStringReference,
                    arguments: new object[]
                    {
                        // 0
                        movementSpeed,
                        movementSpeed - with.movementSpeed,
                        // 2 
                        rotationSpeed,
                        rotationSpeed - with.rotationSpeed,
                        // 4
                        maxHealth,
                        maxHealth - with.maxHealth,
                        // 6
                        attackPower,
                        attackPower - with.attackPower,
                        // 8
                        attackDamage,
                        attackDamage - with.attackDamage,
                        // 10
                        armor,
                        armor - with.armor,
                        // 12
                        abilityDmg,
                        abilityDmg - (int) (with.abilityDamage * 100),
                        // 14
                        muta,
                        muta - (int)(with.mutagenicity * 100),
                        // 16
                        bonusSummonAmount,
                        bonusSummonAmount - with.bonusSummonAmount,
                        // 18
                        summonDmg,
                        summonDmg - (int) (with.summonDamage * 100)
                    });
        }
    }
}