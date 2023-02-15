using Definitions;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class PopupNotification : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Text popupText;

        private INotificationProvider provider;
        private float distanceToPlayer;
        private Vector2 providerStaticPosition;
        private bool isActive;

        private static readonly int PopoutHash = Animator.StringToHash("PopupNotificationPopout");
        private static readonly int PopupHash = Animator.StringToHash("PopupNotificationPopup");
        
        
        
        public void SetDataProvider(INotificationProvider target)
        {
            provider = target;
            provider.OnDataUpdate += UpdateText;
            provider.OnProviderDestroy += OnProviderDestroy;
            providerStaticPosition = provider.Position;
            transform.localPosition = providerStaticPosition;
            UpdateText();
        }

        private void UpdateText() => popupText.text = provider.NotificationText;

        private void OnProviderDestroy()
        {
            provider.OnDataUpdate -= UpdateText;
            provider.OnProviderDestroy -= OnProviderDestroy;
            provider = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if(provider is null) return;
            provider.OnDataUpdate -= UpdateText;
            provider.OnProviderDestroy -= OnProviderDestroy;
            provider = null;
        }

        private void Update()
        {
            distanceToPlayer = (providerStaticPosition - Player.Movement.Position).sqrMagnitude;
            SetActive(distanceToPlayer <= GlobalDefinitions.InteractionDistance);
        }

        private void SetActive(bool activeState)
        {
            if (isActive == activeState) return;
            if (activeState)
            {
                animator.gameObject.SetActive(true);
                animator.Play(PopupHash);
            }
            else animator.Play(PopoutHash);
            isActive = activeState;
        }
    }
}