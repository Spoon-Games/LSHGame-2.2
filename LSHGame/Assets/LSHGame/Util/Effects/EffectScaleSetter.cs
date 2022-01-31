using UnityEngine;

namespace LSHGame.Util
{
    public class EffectScaleSetter : MonoBehaviour, IEffectPlayer
    {
        public bool applyGlobaly;

        public bool ignoreX;
        public bool ignoreY;
        public bool ignoreZ;

        public void Play(Bundle parameters)
        {

            if(parameters != null && parameters.TryGet<Vector3>("Scale",out Vector3 scale))
            {
                if (applyGlobaly && transform.parent != null)
                    scale = scale.Divide(transform.parent.lossyScale);

                if (ignoreX)
                    scale.x = 1;

                if (ignoreY)
                    scale.y = 1;

                if (ignoreZ)
                    scale.z = 1;

                transform.localScale = scale;
            }
        }

        public void Stop()
        {
            
        }
    }
}
