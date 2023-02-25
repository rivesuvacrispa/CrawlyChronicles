using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class VsyncToggleButton : ToggleButton
    {
        [SerializeField] private Slider fpsSlider;
        [SerializeField] private Text fpsText;
        [SerializeField] private Image fpsFillArea;


        protected override void OnToggle(bool toggled)
        {
            base.OnToggle(toggled);
            QualitySettings.vSyncCount = toggled ? 1 : 0;
            fpsSlider.interactable = !toggled;
            fpsText.color = fpsText.color.WithAlpha(toggled ? 0.25f : 1f);
            fpsFillArea.color = fpsFillArea.color.WithAlpha(toggled ? 0.25f : 1f);
        }
    }
}