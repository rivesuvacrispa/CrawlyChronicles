using Definitions;
using Scriptable;
using SoundEffects;
using UI.Elements;
using UI.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Util;

namespace UI.Menus
{
    public class SettingsMenu : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool debug_OverrideDifficulty;
        public OverallDifficulty debug_Difficulty;
#endif
        [SerializeField] private Difficulty[] difficulties = new Difficulty[3];
        [SerializeField] private DifficultyButton[] difficultyButtons = new DifficultyButton[3];
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
        public delegate void MainMenuDifficultyEvent(Difficulty difficulty);
        public static event MainMenuDifficultyEvent OnDifficultyChanged;
        
        public static Difficulty SelectedDifficulty { get; private set; }
        
        private void Awake()
        {
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
            musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
            targetFramerateSlider.onValueChanged.AddListener(UpdateTargetFramerate);
            motionBlurSlider.onValueChanged.AddListener(UpdateMotionBlur);


            ApplySettings(SettingsLoader.CurrentSettings);
            
#if UNITY_EDITOR
            if (debug_OverrideDifficulty) SelectedDifficulty = GetDifficulty(debug_Difficulty);
#endif
        }

        private Difficulty GetDifficulty(OverallDifficulty difficulty) => difficulties[(int) difficulty];
        
        public void ApplyGameDifficulty()
        {
            var diff = DifficultyButton.SelectedDifficulty;
            if(diff is null) diff = GetDifficulty(OverallDifficulty.Affordable);
            OnDifficultyChanged?.Invoke(diff);

            SelectedDifficulty = diff;
            SettingsLoader.CurrentSettings.Difficulty = diff.OverallDifficulty;
            SettingsLoader.SaveSettings(SettingsLoader.CurrentSettings);
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
            int diff = (int) settings.Difficulty;
            difficultyButtons[diff].Select();
            SelectedDifficulty = difficulties[diff];
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
                motionBlurSlider.value,
                SelectedDifficulty.OverallDifficulty);
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