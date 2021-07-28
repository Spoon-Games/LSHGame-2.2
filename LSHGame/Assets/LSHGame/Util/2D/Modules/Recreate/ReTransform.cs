using UnityEngine;

namespace LSHGame.Util
{
    public class ReTransform : MonoBehaviour, IRecreatable
    {
        private enum RecreateOption { Global, No, Local}

        [SerializeField] private RecreateOption Position = RecreateOption.Global;
        [SerializeField] private RecreateOption Rotation = RecreateOption.Global;
        [SerializeField] private RecreateOption Scale = RecreateOption.Local;

        private Vector3 initPosition;
        private Quaternion initRotation;
        private Vector3 initScale;

        private void Awake()
        {
            initPosition = Position == RecreateOption.Local ? transform.localPosition : transform.position;
            initRotation = Rotation == RecreateOption.Local ? transform.localRotation : transform.rotation;
            initScale = Scale == RecreateOption.Local ? transform.localScale : transform.lossyScale;
        }

        public void Recreate()
        {
            switch (Position)
            {
                case RecreateOption.Global: transform.position = initPosition; break;
                case RecreateOption.Local: transform.localPosition = initPosition; break;
            }

            switch (Rotation)
            {
                case RecreateOption.Global: transform.rotation = initRotation; break;
                case RecreateOption.Local: transform.localRotation = initRotation; break;
            }

            switch (Scale)
            {
                case RecreateOption.Global:
                    if (transform.parent == null)
                        transform.localScale = initScale;
                    else
                        transform.localScale = initScale.Divide(transform.parent.lossyScale);
                        break;
                case RecreateOption.Local: transform.localScale = initScale; break;
            }
        }
    }
}