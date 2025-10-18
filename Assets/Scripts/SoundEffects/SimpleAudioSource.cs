using Definitions;
using UI.Menus;
using UnityEngine;
using Util;

namespace SoundEffects
{
    public class SimpleAudioSource : MonoBehaviour
    {
        [SerializeField] private bool pauseable;
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;
        
        public void Play(bool loop = false, float pitch = 1f)
        {
            source.clip = clip;
            source.loop = loop;
            source.pitch = pitch;
            source.Play();
        }

        private void OnVolumeChanged(float volume)
        {
            source.volume = volume;
        }

        private void Awake()
        {
            SettingsMenu.OnSFXVolumeChanged += OnVolumeChanged;
            OnVolumeChanged(SettingsLoader.CurrentSettings.SfxVolume);
            if (pauseable) PauseTrigger.OnPauseTriggered += OnPauseTriggered;
        }

        private void OnPauseTriggered(bool isPaused)
        {
            if (isPaused)
                source.Pause();
            else
                source.UnPause();
        }

        private void OnDestroy()
        {
            SettingsMenu.OnSFXVolumeChanged -= OnVolumeChanged;
            if (pauseable) PauseTrigger.OnPauseTriggered -= OnPauseTriggered;
        }
    }
}