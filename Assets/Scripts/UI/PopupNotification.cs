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
        private Vector2 providerStaticPosition;
        private bool isActive;
        private bool isStatic;
        private Transform cachedTransfrom;

        private static readonly int PopoutHash = Animator.StringToHash("PopupNotificationPopout");
        private static readonly int PopupHash = Animator.StringToHash("PopupNotificationPopup");
        
        
        
        public PopupNotification SetDataProvider(INotificationProvider target, bool staticProvider)
        {
            provider = target;
            provider.OnDataUpdate += UpdateText;
            provider.OnProviderDestroy += OnProviderDestroy;
            cachedTransfrom = target.Transform;
            isStatic = staticProvider;
            providerStaticPosition = cachedTransfrom.position;
            transform.localPosition = providerStaticPosition;
            animator.gameObject.SetActive(false);
            UpdateText();
            return this;
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
            // TODO: This def can be rewritten
            Vector2 positionToUse = isStatic ? providerStaticPosition : cachedTransfrom.position;
            float distanceToPlayer = (positionToUse - Player.Movement.Position).sqrMagnitude;
            SetActive(distanceToPlayer <= GlobalDefinitions.InteractionDistance);

            if (!isStatic) transform.localPosition = cachedTransfrom.position;
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