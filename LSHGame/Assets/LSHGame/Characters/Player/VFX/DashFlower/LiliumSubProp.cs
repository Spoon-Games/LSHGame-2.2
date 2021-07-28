using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(RecreateModule))]
    public class LiliumSubProp : SubstanceProperty, IRecreatable
    {
        [SerializeField]
        private ParticleSystem liliumParticleSystem;
        public ParticleSystem LiliumParticleSystem => liliumParticleSystem;

        private bool hasLilium = true;
        public bool HasLilium => hasLilium;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is ILiliumReciever r && HasLilium)
            {
                r.LiliumReference = this;
            }
        }


        public bool GetLilium()
        {
            if (!HasLilium)
                return false;

            hasLilium = false;
            return true;
        }

        public void Recreate()
        {
            hasLilium = true;
            if (!LiliumParticleSystem.isPlaying)
                LiliumParticleSystem.Play();
        }
    }

    public interface ILiliumReciever
    {
        LiliumSubProp LiliumReference { get; set; }
    }
}
