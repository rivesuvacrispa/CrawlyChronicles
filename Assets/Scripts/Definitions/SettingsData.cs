using UnityEngine;

namespace Definitions
{
    [System.Serializable]
    public class SettingsData
    {
        [SerializeField] private float sfxVolume;
        [SerializeField] private float musicVolume;
        [SerializeField] private int targetFramerate;
        [SerializeField] private bool vSync;
        [SerializeField] private bool ambient;
        [SerializeField] private bool fpsCounter;

        public float SfxVolume => sfxVolume;
        public float MusicVolume => musicVolume;
        public int TargetFramerate => targetFramerate;
        public bool VSync => vSync;
        public bool Ambient => ambient;
        public bool FpsCounter => fpsCounter;

        public SettingsData(float sfxVolume, float musicVolume, int targetFramerate, bool vSync, bool ambient, bool fpsCounter)
        {
            this.sfxVolume = sfxVolume;
            this.musicVolume = musicVolume;
            this.targetFramerate = targetFramerate;
            this.vSync = vSync;
            this.ambient = ambient;
            this.fpsCounter = fpsCounter;
        }

        public static SettingsData Default() => new
                (100,
                100,
                60,
                false,
                true,
                true);
    }
}