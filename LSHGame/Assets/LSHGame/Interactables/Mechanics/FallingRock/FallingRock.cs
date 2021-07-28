using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(RecreateModule))]
    [RequireComponent(typeof(ReRigidbody2D))]
    public class FallingRock : MonoBehaviour, IRecreatable
    {
        [SerializeField]
        private float stallTime = 1;

        [SerializeField]
        private float gravityModifier = 1f;

        [SerializeField]
        private LayerMask playerLayers;

        private Animator animator;
        private EffectsController effectsController;
        private Rigidbody2D rb;

        private enum RockStates { Idle, Stalling, Falling, Landed }
        private RockStates state = RockStates.Idle;

        private float fallingVelocity = 0;
        private Timer impactThreasholdTimer = new Timer() { durration = 0.2f };

        private bool wasDestroyed = false;

        protected void Awake()
        {
            animator = GetComponent<Animator>();
            effectsController = GetComponent<EffectsController>();
            rb = GetComponent<Rigidbody2D>();
        }


        public void ActivateRock()
        {
            if (state == RockStates.Idle)
            {
                state = RockStates.Stalling;

                if (stallTime <= 0)
                {
                    StartFallRock();
                }

                animator.SetBool("IsStalling", true);
                effectsController.TriggerEffect("StallingEffect");
                TimeSystem.Delay(stallTime, (t) => StartFallRock(), true);
            }
        }

        private void StartFallRock()
        {
            state = RockStates.Falling;

            animator.SetBool("IsStalling", false);
            effectsController.StopEffect("StallingEffect");
            effectsController.TriggerEffect("FallingEffect");
            impactThreasholdTimer.Clock();
        }

        private void FixedUpdate()
        {
            if(state == RockStates.Falling)
            {
                fallingVelocity += gravityModifier * Physics2D.gravity.y * Time.fixedDeltaTime * Time.fixedDeltaTime;
                Vector2 position = rb.position;
                position.y += fallingVelocity * Time.fixedDeltaTime;
                rb.MovePosition(position);
            }

            if (state == RockStates.Landed && impactThreasholdTimer.Finished)
            {
                PlayImpactEffects();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(state == RockStates.Falling)
            {
                if (playerLayers.IsLayer(collision.collider.gameObject.layer))
                {
                    PlayImpactEffects();
                }
                else
                {
                    state = RockStates.Landed;

                    PlayImpactEffects();
                }
            }
        }

        private void PlayImpactEffects()
        {
            if (impactThreasholdTimer.Finished)
            {
                impactThreasholdTimer.Reset();
                effectsController.StopEffect("FallingEffect");
                effectsController.TriggerEffect("ImpactEffect");
            }
        }

        private void OnDestroy()
        {
            wasDestroyed = true;
        }

        public void Recreate()
        {
            if (!wasDestroyed)
            {
                animator.SetBool("IsStalling", false);
                effectsController.StopEffect("StallingEffect");
                effectsController.StopEffect("FallingEffect");
                state = RockStates.Idle;
                impactThreasholdTimer.Reset();
                fallingVelocity = 0;
            }
        }
    }
}
