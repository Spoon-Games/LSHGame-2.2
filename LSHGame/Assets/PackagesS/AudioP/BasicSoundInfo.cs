using UnityEngine;
using UnityEngine.Audio;

namespace AudioP
{
    [CreateAssetMenu(fileName = "SoundInfo", menuName = "AudioP/BasicSoundInfo", order = 0)]
    public class BasicSoundInfo : RawValueSoundInfo
    {
        [SerializeField]
        private AudioClip audioClip;


        protected override void SetUpValues(AudioSource audioSource)
        {
            audioSource.clip = audioClip;
            base.SetUpValues(audioSource);
        }
    }

    public abstract class RawValueSoundInfo : SoundInfo
    {

        [Range(0, 1)]
        [SerializeField]
        private float volume = 1;

        [Range(0.3f, 3)]
        [SerializeField]
        private float pitch = 1;

        [SerializeField]
        private bool loop = false;

        [SerializeField]
        [Range(-1, 1)]
        private float panStereo = 0;

        [SerializeField]
        [Range(0, 1)]
        private float spatialBlend = 0;

        [SerializeField]
        private float minDistance = 0.5f;

        [SerializeField]
        private float maxDistance = 15f;

        [SerializeField]
        private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [SerializeField]
        private AudioMixerGroup outputGroup;


        protected internal override void PlayRaw(AudioSource audioSource)
        {
            SetUpValues(audioSource);
        }

        protected virtual void SetUpValues(AudioSource audioSource)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.panStereo = panStereo;
            audioSource.spatialBlend = spatialBlend;
            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;
            audioSource.rolloffMode = rolloffMode;

            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = outputGroup;
        }
    }
}

