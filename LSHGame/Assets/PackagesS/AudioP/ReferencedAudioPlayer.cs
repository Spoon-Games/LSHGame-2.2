namespace AudioP
{
    public class ReferencedAudioPlayer : BaseAudioPlayer
    {
        public override void Play()
        {
            AudioManager.Play(soundInfo);
        }

        public override void Stop()
        {
            AudioManager.Stop(soundInfo);
        }
    }
}
