﻿using UnityEngine;
using UnityEngine.Audio;

namespace AudioP
{
    [CreateAssetMenu(fileName = "SoundInfo", menuName = "AudioP/RandomSoundInfo", order = 0)]
    public class RandomSoundInfo : SoundInfo
    {
        public override AudioClip GetAudioClip => audioClips[0];

        [Header("Sounds to play")]
        [SerializeField]
        private AudioClip[] audioClips = new AudioClip[0];

        [SerializeField]
        private SoundInfo[] soundInfos = new SoundInfo[0];

        [SerializeField]
        private int[] propabilities = new int[0]; 

        [Header("Global Settings")]
        [SerializeField]
        private AudioMixerGroup outputGroup;

        [SerializeField]
        [Range(0, 1)]
        private float minVolume = 1;

        [SerializeField]
        [Range(0, 1)]
        private float maxVolume = 1;

        [SerializeField]
        [Range(0.3f, 3)]
        private float minPitch = 1;

        [SerializeField]
        [Range(0.3f, 3)]
        private float maxPitch = 1;

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

        public enum ChildSettingModes { Combine, Override, UseChild }
        [SerializeField]
        private ChildSettingModes childSettingMode;

        protected internal override void PlayRaw(AudioSource audioSource)
        {
            int count = audioClips.Length + soundInfos.Length;

            if (count == 0)
            {
                Debug.LogWarning("No sound was assigned. Unable to play!");
                return;
            }

            foreach (var p in propabilities)
                count += p;

            int selected = Random.Range(0, count);

            for (int i = 0; i < propabilities.Length; i++)
            {
                var p = propabilities[i];
                selected -= p;

                if (selected <= i)
                {
                    selected = i;
                    break;
                }
            }

            if (selected < audioClips.Length)
            {
                SetUpValues(audioSource, false);

                audioSource.clip = audioClips[selected];
            }
            else
            {
                selected -= audioClips.Length;

                soundInfos[selected].PlayRaw(audioSource);

                switch (childSettingMode)
                {
                    case ChildSettingModes.Combine:
                        SetUpValues(audioSource, true);
                        break;
                    case ChildSettingModes.Override:
                        SetUpValues(audioSource, false);
                        break;
                }
            }


            audioSource.outputAudioMixerGroup = outputGroup;


        }

        private void SetUpValues(AudioSource audioSource, bool avarage)
        {
            if (avarage)
            {
                audioSource.volume = (Random.Range(minVolume, maxVolume) + audioSource.volume) / 2;
                audioSource.pitch = (Random.Range(minPitch, maxPitch) + audioSource.pitch) / 2;
                audioSource.panStereo = (panStereo + audioSource.panStereo) / 2;
                audioSource.spatialBlend = (spatialBlend + audioSource.spatialBlend) / 2;

                audioSource.minDistance = (minDistance + audioSource.minDistance) / 2;
                audioSource.maxDistance = (maxDistance + audioSource.maxDistance) / 2;
            }
            else
            {
                audioSource.volume = Random.Range(minVolume, maxVolume);
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.panStereo = panStereo;
                audioSource.spatialBlend = spatialBlend;

                audioSource.minDistance = minDistance;
                audioSource.maxDistance = maxDistance;
            }

            audioSource.rolloffMode = rolloffMode;
            audioSource.loop = loop;
        }
    }
}
