using UnityEngine;

namespace AudioP
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : BaseAudioPlayer
    {
        private AudioSource audioSource;

        [SerializeField]
        private bool waitTillStop = false;
        [SerializeField]
        private float playAgainThreshold = 0.1f;
        [SerializeField]
        private bool stopWhenPlaying = true;
        [SerializeField] private bool disableStopping = false;

        private float lastPlayTimer = float.NegativeInfinity;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void Play()
        {
            if (!waitTillStop || !audioSource.isPlaying || Time.time >= lastPlayTimer) {
                base.soundInfo.Play(audioSource,stopPrevious: stopWhenPlaying);
                lastPlayTimer = Time.time + playAgainThreshold;
            }
        }

        public override void Stop()
        {
            if(!disableStopping)
                audioSource.Stop();
        }

    }

    public abstract class BaseAudioPlayer : MonoBehaviour
    {
        public SoundInfo soundInfo;

        public bool playOnStart = false;

        public bool stopOnDestroy = false;

        protected virtual void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        protected virtual void OnDestroy()
        {
            if (stopOnDestroy)
            {
                Stop();
            }
        }

        public abstract void Play();

        public abstract void Stop();
    }
}
