using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PinchCollider : MonoBehaviour
    {
        private PinchEffector[] pinchEffectors;
        private BoxCollider2D col;

        private void Awake()
        {
            pinchEffectors = GetComponentsInChildren<PinchEffector>();
            col = GetComponent<BoxCollider2D>();
        }

        public Vector2 GetPinchOfVelocity()
        {
            Vector2 dir = Vector2.zero;
            foreach(var effector in pinchEffectors)
            {
                dir += effector.GetPinchForceDirection();
            }

            //Debug.Log("PiCol Vel: " + dir * pinchOfSpeed + " " + name);
            return dir;
        }

        public Rect GetColliderRect() => col.GetColliderRect();

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}
