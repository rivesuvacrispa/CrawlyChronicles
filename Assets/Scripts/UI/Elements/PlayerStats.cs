using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class PlayerStats : MonoBehaviour 
    {
        [SerializeField] private Text statsText;

        
        
        private void OnEnable() => PlayerManager.OnStatsChanged += OnPlayerStatsChanged;

        private void OnDisable() => PlayerManager.OnStatsChanged -= OnPlayerStatsChanged;

        private void OnPlayerStatsChanged() => statsText.text = PlayerManager.PlayerStats.Print();
    }
}