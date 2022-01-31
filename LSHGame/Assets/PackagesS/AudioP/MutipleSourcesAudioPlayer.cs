using UnityEngine;

namespace AudioP
{
    public class MutipleSourcesAudioPlayer : BaseAudioPlayer
    {
        [SerializeField] private AudioSource[] audioSources;
        [SerializeField] private bool overrideLastWhenNoSourceAvailable = true;

        [SerializeField] private float overrideStereoPan = -2;

        public override void Play()
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                var source = audioSources[i];

                if (!source.isPlaying)
                {
                    Play(source);
                    break;
                }
                if(overrideLastWhenNoSourceAvailable && i == audioSources.Length - 1)
                {
                    Play(source);
                }
            }
        }

        private void Play(AudioSource source)
        {
            soundInfo.Play(source);
            if(overrideStereoPan >= -1 && overrideStereoPan <= 1)
            {
                source.panStereo = overrideStereoPan;
            }
        }

        public override void Stop()
        {
            foreach (var source in audioSources)
            {
                source.Stop();
            }
        }
    }
}
