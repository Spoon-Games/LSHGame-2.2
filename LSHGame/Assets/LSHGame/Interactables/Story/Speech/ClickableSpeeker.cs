using LSHGame.UI;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    [RequireComponent(typeof(Collider2D))]
    public class ClickableSpeeker : Speeker
    {
        [SerializeField]
        public GlobalInputAgent interactAgent;

        [SerializeField]
        public string speekText = "Drücke [Leertaste], um zu reden";

        [SerializeField]
        protected LayerMask layerMask;

        private Collider2D triggerCollider;
        private bool isActive = false;

        protected override void Awake()
        {
            base.Awake();
            triggerCollider = GetComponent<Collider2D>();
            interactAgent.Initialize();
        }

        private void OnInteract()
        {
            if (isActive)
            {
                base.Show();
                HelpTextView.Instance.HideHelpText(gameObject);
                interactAgent.StopListening();
            }
        }

        private void FixedUpdate()
        {
            bool newActive = Physics2D.IsTouchingLayers(triggerCollider, layerMask);
            if(newActive != isActive)
            {
                isActive = newActive;

                if (isActive)
                {
                    HelpTextView.Instance.SetHelpText(speekText,gameObject);
                    interactAgent.Listen();
                    interactAgent.Jump.OnPress += OnInteract;
                }
                else
                {
                    HelpTextView.Instance.HideHelpText(gameObject);
                    interactAgent.StopListening();
                    interactAgent.Jump.OnPress -= OnInteract;

                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            interactAgent.StopListening();
            interactAgent.Jump.OnPress -= OnInteract;
        }
    }
}
