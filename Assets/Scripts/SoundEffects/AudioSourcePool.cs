using System.Linq;
using Definitions;
using UI;
using UI.Menus;
using UnityEngine;
using Util;

namespace SoundEffects
{
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField] private bool pauseable;
        [SerializeField, Range(2, 8)] private int poolSize;

        private AudioSource[] pool;

        private void Awake()
        {
            pool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.priority = 0;
                pool[i] = source;
            }
            SettingsMenu.OnSFXVolumeChanged += OnVolumeChanged;
            OnVolumeChanged(SettingsLoader.CurrentSettings.SfxVolume);
            if (pauseable) PauseTrigger.OnPauseTriggered += OnPauseTriggered;
        }

        public void Play(AudioClip clip, float pitch = 1)
        {
            var source = pool.FirstOrDefault(audioSource => !audioSource.isPlaying);
            source ??= pool[0];
            source.clip = clip;
            source.pitch = pitch;
            source.Play();
        }
        
        private void OnVolumeChanged(float volume)
        {
            foreach (AudioSource source in pool) source.volume = volume;
        }
        
        
        private void OnPauseTriggered(bool isPaused)
        {
            foreach (AudioSource source in pool)
            {
                if(isPaused) source.Pause();
                else source.UnPause();
            }
        }
        
        private void OnDestroy()
        {
            SettingsMenu.OnSFXVolumeChanged -= OnVolumeChanged;
            if (pauseable) PauseTrigger.OnPauseTriggered -= OnPauseTriggered;
        }
    }
}