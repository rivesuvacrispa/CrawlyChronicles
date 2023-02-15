using Definitions;
using Gameplay.AI.Locators;
using Gameplay.Food;
using UnityEngine;
using Util;

namespace Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EggBed : MonoBehaviour, INotificationProvider, ILocatorTarget, IFoodBed
    {
        [SerializeField] private int eggsAmount;
        private SpriteRenderer spriteRenderer;


        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        private void Start()
        {
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(eggsAmount);
            GlobalDefinitions.CreateNotification(this);
            BreedingManager.Instance.AddTotalEggsAmount(eggsAmount);
        }

        private void OnDestroy()
        {
            BreedingManager.Instance.AddTotalEggsAmount(-eggsAmount);
            OnProviderDestroy?.Invoke();
        }

        public void RemoveOne()
        {
            int newAmount = eggsAmount - 1;
            if(newAmount <= 0) Destroy(gameObject);
            else
            {
                SetAmount(newAmount);
                BreedingManager.Instance.AddTotalEggsAmount(-1);
            }
        }
        
        public void SetAmount(int amount)
        {
            eggsAmount = amount;
            spriteRenderer.sprite = GlobalDefinitions.GetEggsBedSprite(amount);
            OnDataUpdate?.Invoke();
        }
        
        
        
        // INotificationProvider 
        public event INotificationProvider.NotificationProviderEvent OnDataUpdate;
        public event INotificationProvider.NotificationProviderEvent OnProviderDestroy;
        public Vector2 Position => transform.position;
        public string NotificationText => eggsAmount.ToString();
        public string LocatorTargetName => "EggBed";
    }
}