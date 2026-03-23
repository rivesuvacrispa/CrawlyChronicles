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


        private void OnEnable()
        {
            // localizedString.StringChanged += UpdateText;
            PlayerManager.OnStatsChanged += OnPlayerStatsChanged;
        }

        private void OnDisable()
        {
            // localizedString.StringChanged -= UpdateText;
            PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;
        }

        private void UpdateText(string value)
        {
            localizedString.Arguments = PlayerManager.PlayerStats.GetStringArguments();
            statsText.text = value;
        }

        private void OnPlayerStatsChanged(Gameplay.Player.PlayerStats changes)
        {
            currentArgs = PlayerManager.PlayerStats.GetStringArguments();
            localizedString.Arguments = currentArgs;
            statsText.text = PlayerManager.PlayerStats.Print(currentArgs);
        }
    }
}