using UnityEngine;

namespace LSHGame.Util
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ReRigidbody2D : MonoBehaviour,IRecreatable
    {
        [SerializeField] private bool recreatePosition = true;
        [SerializeField] private bool recreateRotation = true;

        private Rigidbody2D rb;

        private Vector2 initPosition;
        private float initRotation;

        public void Recreate()
        {
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            if (recreatePosition)
                rb.position = initPosition;
            if (recreateRotation)
                rb.rotation = initRotation;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            initPosition = rb.position;
            initRotation = rb.rotation;
        }
    }
}
