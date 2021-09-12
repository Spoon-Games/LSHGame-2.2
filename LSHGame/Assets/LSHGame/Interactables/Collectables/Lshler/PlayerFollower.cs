using LSHGame.PlayerN;
using UnityEngine;

namespace LSHGame
{
    public class PlayerFollower : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private float cirleRadius = 1.5f;

        [SerializeField]
        private float cirleSpeed = 1f;

        [SerializeField]
        private float smoothTime = 1;

        [SerializeField]
        private float maxVelocity = 5;

        [SerializeField]
        private bool active = false;
        public bool Active { get => active;
            set {
                if (active != value)
                {
                    active = value;
                    if (!active)
                    {
                        velocity = Vector2.zero;
                    }
                }
            } }

        private Vector2 velocity = Vector2.zero;

        private Transform target;

        private void Awake()
        {
            target = Player.Instance.transform;
        }

        private void Update()
        {
            if (active)
            {
                Vector2 t = target.position; ;
                t += offset;

                t += (Vector2) (Quaternion.Euler(0, 0, Mathf.Repeat(Time.time * cirleSpeed, 360)) * new Vector2(cirleRadius, 0));

                transform.position = Vector2.SmoothDamp(transform.position, t, ref velocity, smoothTime, maxVelocity);
            }

        }

    }
}
