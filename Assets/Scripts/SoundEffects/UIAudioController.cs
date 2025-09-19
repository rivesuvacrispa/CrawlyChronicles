using UnityEngine;

namespace SoundEffects
{
    public class UIAudioController : AudioController
    {
        public static UIAudioController Instance { get; private set; }
        
        private UIAudioController () => Instance = this;

        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip selectSound;
        [SerializeField] private AudioClip toggleSound;
        [SerializeField] private AudioClip sliderSound;

        public void PlayHover() => PlayAction(hoverSound);
        public void PlaySelect() => PlayState(selectSound);
        public void PlayToggle() => PlayAction(toggleSound);
        public void PlaySlider() => PlayAction(sliderSound);
    }
}