using System.Collections;
using UnityEngine;

namespace LSHGame.Util
{
    public class DelayedEffectAudioPlayer : EffectAudioPlayer
    {
        public float delay = 1;

        public override void Play(Bundle parameters)
        {
            if(delay > 0)
            {
                StartCoroutine(PlayDelayed(parameters));
            }else
                base.Play(parameters);
        }

        private IEnumerator PlayDelayed(Bundle parameters)
        {
            yield return new WaitForSeconds(delay);

            base.Play(parameters);
        }

        public override void Stop()
        {
            StopAllCoroutines();
            base.Stop();
        }
    }
}
