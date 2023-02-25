using UnityEngine;

namespace UI
{
    public class AmbientToggleButton : ToggleButton
    {
        [SerializeField] private GameObject ambientGO;

        protected override void OnToggle(bool currentState)
        {
            base.OnToggle(currentState);
            ambientGO.SetActive(currentState);
        }
    }
}