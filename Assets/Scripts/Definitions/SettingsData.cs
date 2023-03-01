using UnityEngine;
using Util;

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
        [SerializeField] private float motionBlur;
        [SerializeField] private OverallDifficulty difficulty;

        public float SfxVolume => sfxVolume;
        public float MusicVolume => musicVolume;
        public int TargetFramerate => targetFramerate;
        public bool VSync => vSync;
        public bool Ambient => ambient;
        public bool FpsCounter => fpsCounter;
        public float MotionBlur => motionBlur;
        public OverallDifficulty Difficulty
        {
            get => difficulty;
            set => difficulty = value;
        }

        public SettingsData(float sfxVolume, float musicVolume, int targetFramerate, bool vSync, bool ambient, bool fpsCounter, float motionBlur, OverallDifficulty difficulty)
        {
            this.sfxVolume = sfxVolume;
            this.musicVolume = musicVolume;
            this.targetFramerate = targetFramerate;
            this.vSync = vSync;
            this.ambient = ambient;
            this.fpsCounter = fpsCounter;
            this.motionBlur = motionBlur;
            this.difficulty = difficulty;
        }

        public static SettingsData Default() => new
                (100,
                100,
                60,
                false,
                true,
                true,
                0.5f,
                OverallDifficulty.Affordable);
    }
}