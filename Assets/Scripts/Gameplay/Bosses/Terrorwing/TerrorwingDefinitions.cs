using Scriptable;
using UI;
using UnityEngine;

namespace Scripts.Gameplay.Bosses.Terrorwing
{
    public class TerrorwingDefinitions : MonoBehaviour
    {
        private static TerrorwingDefinitions instance;
        [Header("Stats")] 
        [SerializeField] private FloatBossStat swipeAttackDistance = new();
        [SerializeField] private FloatBossStat contactDamage = new();
        [SerializeField] private FloatBossStat bombardierShootingSpeed = new();
        [SerializeField] private FloatBossStat explosionDamage = new();
        [SerializeField] private FloatBossStat bulletHellDamage = new();
        [SerializeField] private FloatBossStat illusionsFadeTime = new();
        [SerializeField] private FloatBossStat maxHealth = new();
        
        public static float SwipeAttackDistance { get; private set; }
        public static float ContactDamage { get; private set; }
        public static float BombardierShootingSpeed { get; private set; }
        public static float ExplosionDamage { get; private set; }
        public static float BulletHellDamage { get; private set; }
        public static float IllusionsFadeTime { get; private set; }
        public static float MaxHealth { get; private set; }
        
        
        
        private TerrorwingDefinitions() => instance = this;

        private void Awake()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            SwipeAttackDistance = swipeAttackDistance.ChangeDifficulty(difficulty);
            ContactDamage = contactDamage.ChangeDifficulty(difficulty);
            BombardierShootingSpeed = bombardierShootingSpeed.ChangeDifficulty(difficulty);
            ExplosionDamage = explosionDamage.ChangeDifficulty(difficulty);
            BulletHellDamage = bulletHellDamage.ChangeDifficulty(difficulty);
            IllusionsFadeTime = illusionsFadeTime.ChangeDifficulty(difficulty);
            MaxHealth = maxHealth.ChangeDifficulty(difficulty);
        }
        
        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}