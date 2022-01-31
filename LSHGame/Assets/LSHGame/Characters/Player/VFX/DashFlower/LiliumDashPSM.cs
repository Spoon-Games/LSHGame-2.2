using AudioP;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioPlayer))]
    public class LiliumDashPSM : ParticleSystemModifier
    {
        public LiliumSpiralPSM liliumSpiralSystem;

        public bool StartPlay = false;

        private AudioPlayer audioPlayer;

        protected override void Awake()
        {
            base.Awake();

            audioPlayer = GetComponent<AudioPlayer>();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (StartPlay)
            {
                Play();
                StartPlay = false;
            }
        } 
#endif

        public void Play()
        {
            liliumSpiralSystem.TriggerLiliumDash();
            audioPlayer.Play();
        }
    }
}
