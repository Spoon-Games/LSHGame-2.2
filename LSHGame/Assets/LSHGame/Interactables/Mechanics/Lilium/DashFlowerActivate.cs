using LSHGame.PlayerN;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Collider2D))]
    public class DashFlowerActivate : MonoBehaviour
    {
        [SerializeField] private string deadFlowerFX;
        [SerializeField] private string activeFlowerFX;
        [SerializeField] private string highlightFX;

        [SerializeField] private EffectsController effectsController;

        [SerializeField] private LayerMask playerLayer;

        private bool isActive;
        private bool isPlayerInTouch;

        private void Update()
        {
            if(!isActive && Player.Instance.IsLiliumActive)
            {
                isActive = true;
                effectsController.StopEffect(deadFlowerFX);
                effectsController.TriggerEffect(activeFlowerFX);
                if (isPlayerInTouch)
                    effectsController.TriggerEffect(highlightFX);
            }else if(isActive && !Player.Instance.IsLiliumActive)
            {
                isActive = false;
                effectsController.TriggerEffect(deadFlowerFX);
                effectsController.StopEffect(highlightFX);
                effectsController.StopEffect(activeFlowerFX);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playerLayer.IsLayer(collision.gameObject.layer) && !isPlayerInTouch)
            {
                isPlayerInTouch = true;
                if (isActive)
                    effectsController.TriggerEffect(highlightFX);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (playerLayer.IsLayer(collision.gameObject.layer) && isPlayerInTouch)
            {
                isPlayerInTouch = false;
                effectsController.StopEffect(highlightFX);
            }
        }
    }
}
