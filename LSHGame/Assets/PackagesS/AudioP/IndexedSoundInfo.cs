using UnityEngine;

namespace AudioP
{
    [CreateAssetMenu(fileName = "IndexedSoundInfo", menuName = "AudioP/IndexedSoundInfo", order = 0)]
    public class IndexedSoundInfo : RawValueSoundInfo
    {
        [SerializeField]
        private AudioClip[] audioClips;

        public int CurrentIndex;

        public void SetChar(char c)
        {
            c = char.ToUpper(c);

            int i = (int)c;

            i -= 64;

            CurrentIndex = i;
        }


        protected override void SetUpValues(AudioSource audioSource)
        {
            if (CurrentIndex >= 0 && CurrentIndex < audioClips.Length)
            {
                audioSource.clip = audioClips[CurrentIndex];
            }
            else
            {
                audioSource.clip = null;
            }
            base.SetUpValues(audioSource);
        }
    }
}
