using UnityEngine;

namespace AudioP
{
    public abstract class SoundInfo : ScriptableObject
    {
        public virtual AudioClip GetAudioClip { get; }

        public void Play(AudioSource audioSource, bool stopPrevious = true)
        {
            if(stopPrevious)
                audioSource.Stop();

            PlayRaw(audioSource);

            if(audioSource.clip != null)
                audioSource.Play();
        }

        protected internal abstract void PlayRaw(AudioSource audioSource);

    }
}
