using Definitions;
using TMPro;
using UnityEngine;
using Util.Interfaces;

namespace UI.Elements
{
    public class PopupNotification : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text popupText;

        private INotificationProvider provider;
        private Vector2 providerStaticPosition;
        private bool isActive;
        private bool isStatic;
        private bool doNotHide;
        private Transform cachedTransform;

        private static readonly int PopoutHash = Animator.StringToHash("PopupNotificationPopout");
        private static readonly int PopupHash = Animator.StringToHash("PopupNotificationPopup");
        
        
        
        public PopupNotification SetDataProvider(INotificationProvider target, bool staticProvider, bool showAlways)
        {
            doNotHide = showAlways;
            provider = target;
            provider.OnDataUpdate += UpdateText;
            provider.OnProviderDestroy += OnProviderDestroy;
            cachedTransform = target.Transform;
            isStatic = staticProvider;
            providerStaticPosition = cachedTransform.position;
            transform.localPosition = providerStaticPosition;
            if(!showAlways) animator.gameObject.SetActive(false);
            else SetActive(true);
            UpdateText();
            return this;
        }

        private void UpdateText() => popupText.text = provider.NotificationText;

        private void OnProviderDestroy(IDestructionEventProvider target)
        {
            provider.OnDataUpdate -= UpdateText;
            target.OnProviderDestroy -= OnProviderDestroy;
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
            if(doNotHide) return;
            // TODO: This def can be rewritten
            Vector2 positionToUse = isStatic ? providerStaticPosition : cachedTransform.position;
            float distanceToPlayer = (positionToUse - Gameplay.Player.PlayerPhysicsBody.Position).sqrMagnitude;
            SetActive(distanceToPlayer <= GlobalDefinitions.InteractionDistance);

            if (!isStatic) transform.localPosition = cachedTransform.position;
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