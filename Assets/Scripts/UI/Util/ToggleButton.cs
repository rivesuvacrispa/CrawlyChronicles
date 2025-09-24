using SoundEffects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Util
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Image checkMarkImage;
        [SerializeField] private bool defaultState;

        public bool Toggled { get; private set; }
        
        public void SetToggled(bool isToggled)
        {
            Toggled = isToggled;
            OnToggle(isToggled);
        }
        
        public void Toggle()
        {
            UIAudioController.Instance.PlayToggle();
            Toggled = !Toggled;
            OnToggle(Toggled);
        }

        protected virtual void OnToggle(bool currentState)
            => checkMarkImage.enabled = currentState;
    }
}