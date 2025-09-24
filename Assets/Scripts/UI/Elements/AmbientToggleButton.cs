using UI.Util;

namespace UI.Elements
{
    public class AmbientToggleButton : ToggleButton
    {
        public delegate void AmbientToggleButtonEvent(bool state);
        public static event AmbientToggleButtonEvent OnToggled;
        
        
        
        protected override void OnToggle(bool currentState)
        {
            base.OnToggle(currentState);
            OnToggled?.Invoke(currentState);
        }
    }
}