using UI.Util;
using UnityEngine;

namespace UI.Elements
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