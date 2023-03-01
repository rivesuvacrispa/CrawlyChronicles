using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Gameplay.Bosses
{
    public class Bossbar : MonoBehaviour
    {
        public static Bossbar Instance { get; private set; }
        
        [SerializeField] private GameObject rootGO;
        [SerializeField] private Image fillingImage;
        [SerializeField] private Text nameText;
        [SerializeField] private Text healthText;
        [SerializeField] private Gradient gradient;
        [SerializeField] private Animator animator;

        private bool isActive;
        private float currentHealth;
        private float maxHealth;
        private static readonly int OutroAnimHash = Animator.StringToHash("BossbarOutro");
        
        
        private Bossbar() => Instance = this;

        public void SetName(string bossName) => nameText.text = bossName;
        
        public void SetActive(bool state)
        {
            if(isActive == state) return;

            isActive = state;
            if(state)
                rootGO.SetActive(true);
            else animator.Play(OutroAnimHash);
        }

        public void AddMaxHealth(float hp)
        {
            maxHealth += hp;
            currentHealth += hp;
            UpdateHealth();
        }
        
        public void SetMaxHealth(float max)
        {
            maxHealth = max;
            currentHealth = max;
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            float value = Mathf.Clamp01(currentHealth / maxHealth);
            fillingImage.fillAmount = value;
            fillingImage.color = gradient.Evaluate(value);
            healthText.text = $"{(int) currentHealth}/{(int) maxHealth}";
        }

        public void Damage(float dmg)
        {
            if(dmg <= 0) return;
            currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
            UpdateHealth();
            if (currentHealth <= 0.01f) animator.Play(OutroAnimHash);
        }
    }
}