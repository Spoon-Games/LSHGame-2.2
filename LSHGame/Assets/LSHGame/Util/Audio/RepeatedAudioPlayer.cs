using AudioP;
using System.Collections;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(AudioSource))]
    public class RepeatedAudioPlayer : BaseAudioPlayer
    {
        [SerializeField] private RandomRange startDelay;
        [SerializeField] private RandomRange repeatDelay;
        [SerializeField] private int cycles = -1;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }


        public override void Play()
        {
            Stop();
            StartCoroutine(PlayLoop());
        }

        public IEnumerator PlayLoop()
        {
            if (startDelay.NextValue() > 0)
                yield return new WaitForSeconds(startDelay.Value);

            for (int i = 0; i < cycles || cycles < 0; i++)
            {
                soundInfo.Play(audioSource);

                yield return new WaitForSeconds(repeatDelay.NextValue());
            }
        }

        public override void Stop()
        {
            StopAllCoroutines();
            audioSource.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            StopAllCoroutines();
        }
    }
}
