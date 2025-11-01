using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.Player
{
    public class PlayerStats : MonoBehaviour 
    {
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private LocalizedString localizedString;

        private object[] currentArgs;
        
        
        private void UpdateText(string value)
        {
            localizedString.Arguments = PlayerManager.PlayerStats.GetStringArguments();
            statsText.text = value;
        }

        private void OnEnable()
        {
            localizedString.StringChanged += UpdateText;
            PlayerManager.OnStatsChanged += OnPlayerStatsChanged;
        }

        private void OnDisable()
        {
            localizedString.StringChanged -= UpdateText;
            PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;
        }

        private void OnPlayerStatsChanged()
        {
            currentArgs = PlayerManager.PlayerStats.GetStringArguments();
            localizedString.Arguments = currentArgs;
            statsText.text = PlayerManager.PlayerStats.Print(currentArgs);
        }
    }
}