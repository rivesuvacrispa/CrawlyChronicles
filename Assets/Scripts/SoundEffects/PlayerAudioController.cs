using UnityEngine;
using Util;

namespace SoundEffects
{
    public class PlayerAudioController : AudioController
    {
        public static PlayerAudioController Instance { get; private set; }

        [SerializeField] private AudioSourcePool pool;
        [SerializeField, Range(-3, 3)] private float comboPitch;
        [SerializeField] private AudioClip[] attackSounds = new AudioClip[4];
        [SerializeField] private AudioClip interactionSound;
        [SerializeField] private AudioClip genePickupSound;
        [SerializeField] private AudioClip crawlSound;
        [SerializeField] private AudioClip reckoningSound;
        [SerializeField] private AudioClip hitSound;

        public PlayerAudioController() => Instance = this;

        
        
        public void PlayAttack(int combo) => PlayAction(attackSounds[combo]);
        public void PlayCombo() => PlayAction(attackSounds[3], true, comboPitch);
        public void PlayInteract() => PlayAction(interactionSound);
        public void PlayCrawl() => PlayState(crawlSound);
        public void PlayGenePickup() => pool.Play(Instance.genePickupSound, pitch: SoundUtility.GetRandomPitchHigher(0.15f));
        public void PlayReckoning() => pool.Play(reckoningSound);
        public void PlayHit() => pool.Play(hitSound);
    }
}