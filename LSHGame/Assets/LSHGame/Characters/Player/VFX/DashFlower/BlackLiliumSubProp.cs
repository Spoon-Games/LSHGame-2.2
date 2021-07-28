using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(RecreateModule))]
    public class BlackLiliumSubProp : SubstanceProperty,IRecreatable
    {
        private bool isDead = true;
        public bool IsDead => isDead;

        [SerializeField]
        private GameObject blackSprite;
        [SerializeField]
        private GameObject healedSprite;
        [SerializeField]
        private float healTime = 0.1f;

        private bool destroied = false;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is IBlackLiliumReciever r && IsDead)
            {
                r.BlackLiliumReference = this;
            }
        }

        public bool DeliverLilium()
        {
            if (!IsDead)
                return false;

            isDead = false;

            TimeSystem.Delay(healTime, t => Heal());

            return true;
        }

        private void Heal()
        {
            if (destroied)
                return;
            blackSprite.SetActive(false);
            healedSprite.SetActive(true);
        }

        private void OnDestroy()
        {
            destroied = true;
        }

        public void Recreate()
        {
            isDead = true;
            healedSprite.SetActive(false);
            blackSprite.SetActive(true);
        }
    }

    public interface IBlackLiliumReciever
    {
        BlackLiliumSubProp BlackLiliumReference { get; set; }
    }
}
