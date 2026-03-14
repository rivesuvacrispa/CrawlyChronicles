using UnityEngine;

namespace SoundEffects
{
    public class RandomAudioSource : BaseAudioSource
    {
        
        [SerializeField] private AudioClip[] clips;


        protected override AudioClip Clip => clips[Random.Range(0, clips.Length)];
    }
}