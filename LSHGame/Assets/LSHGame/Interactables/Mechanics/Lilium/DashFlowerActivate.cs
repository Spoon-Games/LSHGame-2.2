using LSHGame.PlayerN;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Collider2D))]
    public class DashFlowerActivate : MonoBehaviour
    {
        [SerializeField] private ParticleSystem deadFlowerSystem;
        [SerializeField] private ParticleSystem activeFlowerSystem;
        [SerializeField] private ParticleSystem highlightSystem;

        [SerializeField] private LayerMask playerLayer;

        private bool isActive;
        private bool isPlayerInTouch;

        private void Update()
        {
            if(!isActive && Player.Instance.IsLiliumActive)
            {
                isActive = true;
                deadFlowerSystem.Stop();
                activeFlowerSystem.Play();
                if (isPlayerInTouch)
                    highlightSystem.Play();
            }else if(isActive && !Player.Instance.IsLiliumActive)
            {
                isActive = false;
                deadFlowerSystem.Play();
                activeFlowerSystem.Stop();
                highlightSystem.Stop();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playerLayer.IsLayer(collision.gameObject.layer) && !isPlayerInTouch)
            {
                isPlayerInTouch = true;
                if (isActive)
                    highlightSystem.Play();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (playerLayer.IsLayer(collision.gameObject.layer) && isPlayerInTouch)
            {
                isPlayerInTouch = false;
                highlightSystem.Stop();
            }
        }
    }
}
