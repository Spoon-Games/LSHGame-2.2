using AudioP;
using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(BaseAudioPlayer))]
    public class EffectAudioPlayer : MonoBehaviour, IEffectPlayer
    {
        protected BaseAudioPlayer audioPlayer;

        private void Awake()
        {
            audioPlayer = GetComponent<BaseAudioPlayer>();
        }

        public virtual void Play(Bundle parameters)
        {
            audioPlayer.Play();
        }

        public virtual void Stop()
        {
            audioPlayer.Stop();
        }
    }

}
