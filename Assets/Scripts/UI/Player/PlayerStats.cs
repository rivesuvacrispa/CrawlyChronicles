using Gameplay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class PlayerStats : MonoBehaviour 
    {
        [SerializeField] private TMP_Text statsText;

        
        
        private void OnEnable() => PlayerManager.OnStatsChanged += OnPlayerStatsChanged;

        private void OnDisable() => PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;

        private void OnPlayerStatsChanged() => statsText.text = PlayerManager.PlayerStats.Print();
    }
}