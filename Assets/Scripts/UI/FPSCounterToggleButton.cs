using UnityEngine;

namespace UI
{
    public class FPSCounterToggleButton : ToggleButton
    {
        [SerializeField] private GameObject fpsCounterGO;
        
        protected override void OnToggle(bool currentState)
        {
            base.OnToggle(currentState);
            fpsCounterGO.SetActive(currentState);
        }
    }
}