using Scriptable;
using UI;
using UnityEngine;

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
        [SerializeField] private float fragmentSpeed = 4;
        [SerializeField] private float followRadius;
        [SerializeField] private Gradient centipedeGradient;
        [SerializeField] private float followRotationSpeed;


        public static int BodyLength { get; private set; }
        public static float FollowSpeed { get; private set; }
        public static float FragmentHealth { get; private set; }
        public static float ContactDamage { get; private set; }
        public static float AttackDamage { get; private set; }
        public static float PoisonDamage { get; private set; }
        public static float Knockback { get; private set; }
        public static float Armor { get; private set; }
        public static GameObject FragmentPrefab => instance.centipedeFragmentPrefab;
        public static float FollowRotationSpeed => instance.followRotationSpeed;
        public static float FragmentSpeed => instance.fragmentSpeed;
        public static float FollowRadius => instance.followRadius;


        
        private CentipedeDefinitions() => instance = this;

        private void Awake()
        {
            OnDifficultyChanged(SettingsMenu.SelectedDifficulty);
            SettingsMenu.OnDifficultyChanged += OnDifficultyChanged;
        }

        private void OnDifficultyChanged(Difficulty difficulty)
        {
            ContactDamage = contactDamage.ChangeDifficulty(difficulty);
            AttackDamage = attackDamage.ChangeDifficulty(difficulty);
            PoisonDamage = poisonDamage.ChangeDifficulty(difficulty);
            Knockback = knockback.ChangeDifficulty(difficulty);
            FollowSpeed = followSpeed.ChangeDifficulty(difficulty);
            Armor = armor.ChangeDifficulty(difficulty);
            FragmentHealth = fragmentHealth.ChangeDifficulty(difficulty);
            BodyLength = bodyLength.ChangeDifficulty(difficulty);
        }

        public static Color GetFragmentColor(float value) => instance.centipedeGradient.Evaluate(value);

        private void OnDestroy() => SettingsMenu.OnDifficultyChanged -= OnDifficultyChanged;
    }
}