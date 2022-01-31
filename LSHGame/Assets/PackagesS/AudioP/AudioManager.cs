using SceneM;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioP
{
    public class AudioManager : Singleton<AudioManager>
    {
        public static float MasterVolume { get => Instance.GetExpoFloat("Master_Volume"); set => Instance.SetExpFloat("Master_Volume", value); }
        public static float SFXVolume { get => Instance.GetExpoFloat("SFX_Volume"); set => Instance.SetExpFloat("SFX_Volume", value); }
        public static float MusicVolume { get => Instance.GetExpoFloat("Music_Volume"); set => Instance.SetExpFloat("Music_Volume", value); }

        [SerializeField]
        private AudioMixer mainAudioMixer;

        private Dictionary<SoundInfo, AudioSource> audioSources = new Dictionary<SoundInfo, AudioSource>();

        private GameObject audioSourceParent;

        public override void Awake()
        {
            base.Awake();

            GetAudioSourcesParent(out audioSourceParent);

            if (mainAudioMixer == null)
            {
                Debug.LogError("AudioMixer has to be asigned to AudioManager");
            }
        }

        public static void Play(SoundInfo soundInfo)
        {
            Instance.PlayInstance(soundInfo);
        }

        private void PlayInstance(SoundInfo soundInfo)
        {
            if (!audioSources.TryGetValue(soundInfo, out AudioSource audioSource))
            {
                audioSource = audioSourceParent.AddComponent<AudioSource>();
                audioSources.Add(soundInfo, audioSource);
            }

            soundInfo.Play(audioSource);

        }

        public static void Stop(SoundInfo soundInfo)
        {
            Instance.StopInstance(soundInfo);
        }

        private void StopInstance(SoundInfo soundInfo)
        {
            if(audioSources.TryGetValue(soundInfo,out AudioSource audioSource))
            {
                audioSource.Stop();
            }
        }


        private void GetAudioSourcesParent(out GameObject parent)
        {
            Transform audioSourcesTransform = transform.Find("AudioSources");
            if (audioSourcesTransform == null)
            {
                parent = new GameObject("AudioSources");
                parent.transform.SetParent(transform);
            }
            else
                parent = audioSourcesTransform.gameObject;
        }

        private float GetExpoFloat(string name)
        {
            mainAudioMixer.GetFloat(name, out float v);
            return Mathf.InverseLerp(-80,0,v);
        }

        private void SetExpFloat(string name, float value)
        {
            mainAudioMixer.SetFloat(name, Mathf.Lerp(-80,0,value));
        }
    }
}
