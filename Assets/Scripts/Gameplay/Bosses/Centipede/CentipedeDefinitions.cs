using Scriptable;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Gameplay.Bosses.Centipede
{
    [System.Serializable]
    public class CentipedeDefinitions : MonoBehaviour
    {
        private static CentipedeDefinitions instance;
        [Header("Stats")] 
        [SerializeField] private FloatBossStat contactDamage = new();
        [SerializeField] private FloatBossStat attackDamage = new();
        [SerializeField] private FloatBossStat poisonDamage = new();
        [SerializeField] private FloatBossStat knockback = new();
        [SerializeField] private FloatBossStat followSpeed = new();
        [SerializeField] private FloatBossStat armor = new();
        [SerializeField] private FloatBossStat fragmentHealth = new();
        [SerializeField] private IntBossStat bodyLength = new();

        [Header("Util")]
        [SerializeField] private GameObject centipedeFragmentPrefab;
        [SerializeField] private float speed = 4;
        [SerializeField] private float followRadius = 0.2f;
        [SerializeField] private Gradient centipedeGradient;
        [SerializeField] private float followRotationSpeed;


        public static int BodyLength => instance.bodyLength.CurrentValue;
        public static float FollowSpeed => instance.followSpeed.CurrentValue;
        public static float FragmentHealth => instance.fragmentHealth.CurrentValue;
        public static float ContactDamage => instance.contactDamage.CurrentValue;
        public static float AttackDamage => instance.attackDamage.CurrentValue;
        public static float PoisonDamage => instance.poisonDamage.CurrentValue;
        public static float Knockback => instance.knockback.CurrentValue;
        public static float Armor => instance.armor.CurrentValue;
        
        public static GameObject FragmentPrefab => instance.centipedeFragmentPrefab;
        public static float FollowRotationSpeed => instance.followRotationSpeed;
        public static float Speed => instance.speed;
        public static float FollowRadius => instance.followRadius;


        private CentipedeDefinitions() => instance = this;

        private void Start()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            contactDamage.ChangeDifficulty(difficulty);
            attackDamage.ChangeDifficulty(difficulty);
            poisonDamage.ChangeDifficulty(difficulty);
            knockback.ChangeDifficulty(difficulty);
            followSpeed.ChangeDifficulty(difficulty);
            armor.ChangeDifficulty(difficulty);
            fragmentHealth.ChangeDifficulty(difficulty);
            bodyLength.ChangeDifficulty(difficulty);
        }

        public static Color GetFragmentColor(float value) => instance.centipedeGradient.Evaluate(value);

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}