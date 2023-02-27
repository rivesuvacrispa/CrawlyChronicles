using Definitions;
using Scripts.SoundEffects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("UI refs")]
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider targetFramerateSlider;
        [SerializeField] private Slider motionBlurSlider;
        [SerializeField] private ToggleButton vSyncToggleButton;
        [SerializeField] private ToggleButton ambientToggleButton;
        [SerializeField] private ToggleButton fpsCounterToggleButton;
        [Header("Other refs")]
        [SerializeField] private Volume worldVolume;
        
        public delegate void SettingsMenuEvent(float value);
        public static SettingsMenuEvent OnSFXVolumeChanged;
        public static SettingsMenuEvent OnMusicVolumeChanged;

        
        private void Start()
        {
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
            targetFramerateSlider.onValueChanged.AddListener(UpdateTargetFramerate);
            motionBlurSlider.onValueChanged.AddListener(UpdateMotionBlur);
            
            ApplySettings(SettingsLoader.CurrentSettings);
        }

        private void ApplySettings(SettingsData settings)
        {
            sfxVolumeSlider.value = settings.SfxVolume;
            musicVolumeSlider.value = settings.MusicVolume;
            targetFramerateSlider.value = settings.TargetFramerate;
            motionBlurSlider.value = settings.MotionBlur;
            vSyncToggleButton.SetToggled(settings.VSync);
            ambientToggleButton.SetToggled(settings.Ambient);
            fpsCounterToggleButton.SetToggled(settings.FpsCounter);
        }

        public void CollectSettings()
        {
            SettingsData settings = new SettingsData(
                sfxVolumeSlider.value,
                musicVolumeSlider.value,
                (int) targetFramerateSlider.value,
                vSyncToggleButton.Toggled,
                ambientToggleButton.Toggled,
                fpsCounterToggleButton.Toggled,
                motionBlurSlider.value);
            SettingsLoader.SaveSettings(settings);
        }

        public void ResetSettings()
        {
            SettingsData settings = SettingsData.Default();
            ApplySettings(settings);
            SettingsLoader.SaveSettings(settings);
        }

        private static void UpdateSFXVolume(float value)
        {
            OnSFXVolumeChanged?.Invoke(value);
            UIAudioController.Instance.PlaySlider();
        }

        private static void UpdateMusicVolume(float value)
        {
            OnMusicVolumeChanged?.Invoke(value);
            UIAudioController.Instance.PlaySlider();
        }

        private static void UpdateTargetFramerate(float value)
        {
            Application.targetFrameRate = (int) value;
            UIAudioController.Instance.PlaySlider();
        }

        private void UpdateMotionBlur(float value)
        {
            UIAudioController.Instance.PlaySlider();
            if(worldVolume.profile.TryGet(out MotionBlur motionBlur)) 
                motionBlur.intensity.value = Mathf.Clamp01(value);
        }
    }
}