using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(Collider2D))]
    public class PinchEffector : MonoBehaviour
    {
        [SerializeField] private float pinchOfSpeed = 1;
        [SerializeField] private LayerMask pinchOfLayers;
        [SerializeField] private Timer pinchOfExtensionTimer;

        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        public Vector2 GetPinchForceDirection()
        {
            if (Physics2D.OverlapCollider(col,
                new ContactFilter2D()
                {
                    layerMask = pinchOfLayers,
                    useLayerMask = true,
                    useTriggers = false
                },new Collider2D[1]) > 0)
            {
                //Debug.Log("PiEff: Touch " + name);


                pinchOfExtensionTimer.Clock();
                return (Vector2)transform.up * pinchOfSpeed;
            }

            if (pinchOfExtensionTimer.Active)
                return (Vector2)transform.up * pinchOfSpeed;

            return Vector2.zero;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Color.magenta;

            Gizmos.DrawLine(Vector3.zero, Vector3.up * pinchOfSpeed * pinchOfExtensionTimer.durration);
        }
#endif
    }
}
