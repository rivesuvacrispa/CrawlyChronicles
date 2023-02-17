using Definitions;
using UnityEngine;
using Util;

namespace Gameplay.Food
{
    public class Fungi : FoodBed, INotificationProvider
    {
        [SerializeField] private int amount;

        private void Start()
        {
            GlobalDefinitions.CreateNotification(this);
        }

        public override void Eat()
        {
            amount--;
            if(amount <= 0) Destroy(gameObject);
            else OnDataUpdate?.Invoke();
        }

        public override void OnInteractionStart()
        {
        }

        public override void OnInteractionStop()
        {
        }

        public override bool CanInteract()
        {
            return true;
        }

        private void OnDestroy() => OnProviderDestroy?.Invoke();
        
        
        
        // INotificationProvider
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Transform Transform => transform;
        public string NotificationText => amount.ToString();
    }
}