using Definitions;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Scripts.SoundEffects
{
    public class AudioController : MonoBehaviour
    {
        [FormerlySerializedAs("pausable")] [SerializeField] private bool pauseable;
        [SerializeField] protected AudioSource actionSource;
        [SerializeField] protected AudioSource stateSource;
        
        public void PlayAction(AudioClip clip, bool loop = false, float pitch = 1f)
        {
            actionSource.clip = clip;
            actionSource.loop = loop;
            actionSource.pitch = pitch;
            actionSource.Play();
        }
        
        public void PlayState(AudioClip clip, bool loop = false, float pitch = 1f)
        {
            if(stateSource.isPlaying) return;
            stateSource.clip = clip;
            stateSource.loop = loop;
            stateSource.pitch = pitch;
            stateSource.Play();
        }

        public void StopAction()
        {
            if(!actionSource.isPlaying) return;
            if (actionSource.loop) actionSource.loop = false;
            else actionSource.Stop();
        }
        
        public void StopState()
        {
            if(!stateSource.isPlaying) return;
            stateSource.Stop();
        }

        protected virtual void OnVolumeChanged(float volume)
        {
            stateSource.volume = volume;
            actionSource.volume = volume;
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
            {
                stateSource.Pause();
                actionSource.Pause();
            }
            else
            {
                stateSource.UnPause();
                actionSource.UnPause();
            }
        }

        private void OnDestroy()
        {
            SettingsMenu.OnSFXVolumeChanged -= OnVolumeChanged;
            if (pauseable) PauseTrigger.OnPauseTriggered -= OnPauseTriggered;
        }
    }
}