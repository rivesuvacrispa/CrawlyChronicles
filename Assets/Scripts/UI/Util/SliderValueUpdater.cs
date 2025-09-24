using UnityEngine;
using UnityEngine.UI;

namespace UI.Util
{
    [RequireComponent(typeof(Slider))]
    public class SliderValueUpdater : MonoBehaviour
    {
        [SerializeField] private Text valueText;
        [SerializeField] private bool isPercent;
        
        private void Start()
        {
            var slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(slider.value);
        }

        private void OnValueChanged(float value)
        {
            valueText.text = isPercent ? 
                $"{(int) (value * 100)}%" : 
                $"{(int) value}";
        }
    }
}