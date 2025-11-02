using System;
using Gameplay.AI.Locators;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerLocatorBody : MonoBehaviour, ILocatorTarget
    {
        private static PlayerLocatorBody instance;

        public delegate void PlayerLocatorBodyEvent();

        public static PlayerLocatorBodyEvent OnEnabled;
        public static PlayerLocatorBodyEvent OnDisabled;
        
        public static bool Enabled
        {
            get => instance.gameObject.activeInHierarchy;
            set => instance.gameObject.SetActive(value);
        }

        

        private PlayerLocatorBody() => instance = this;

        private void OnEnable() => OnEnabled?.Invoke();

        private void OnDisable() => OnDisabled?.Invoke();
    }
}