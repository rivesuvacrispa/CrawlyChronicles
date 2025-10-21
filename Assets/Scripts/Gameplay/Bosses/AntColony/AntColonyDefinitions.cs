using Scriptable;
using UI.Menus;
using UnityEngine;

namespace Gameplay.Bosses.AntColony
{
    public class AntColonyDefinitions : MonoBehaviour
    {
        [Header("Stats")] 
        [SerializeField] private FloatBossStat health = new();
        [SerializeField] private FloatBossStat armor = new();
        [SerializeField] private FloatBossStat moveSpeed = new();
        [SerializeField] private FloatBossStat damage = new();
        [SerializeField] private IntBossStat eggsAmount = new();
        [SerializeField] private IntBossStat eggsHatchTime = new();
        [SerializeField] private IntBossStat eggsHealth = new();

        
        
        public static float Health { get; private set; }
        public static float Armor { get; private set; }
        public static float MoveSpeed { get; private set; }
        public static float Damage { get; private set; }
        public static int EggsAmount { get; private set; }
        public static int EggsHatchTime { get; private set; }
        public static int EggsHealth { get; private set; }
        
        
        
        private void Awake()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            Health = health.ChangeDifficulty(difficulty);
            Armor = armor.ChangeDifficulty(difficulty);
            MoveSpeed = moveSpeed.ChangeDifficulty(difficulty);
            Damage = damage.ChangeDifficulty(difficulty);
            EggsAmount = eggsAmount.ChangeDifficulty(difficulty);
            EggsHatchTime = eggsHatchTime.ChangeDifficulty(difficulty);
            EggsHealth = eggsHealth.ChangeDifficulty(difficulty);
        }

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}