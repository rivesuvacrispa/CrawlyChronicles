using UnityEngine;

namespace SoundEffects
{
    public class SimpleAudioSource : BaseAudioSource
    {

        [SerializeField] private AudioClip clip;


        protected override AudioClip Clip => clip;
    }
}