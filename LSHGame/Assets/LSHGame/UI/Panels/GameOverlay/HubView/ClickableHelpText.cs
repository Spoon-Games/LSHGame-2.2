using LSHGame.Util;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.UI
{
    public class ClickableHelpText : ShowHelpTextComponent
    {
        public UnityEvent OnInteract;

        public GlobalInputAgent inputAgent;


        protected override void OnShow()
        {
            base.OnShow();

            inputAgent.Listen();
            inputAgent.Jump.OnPress += Interact;
        }

        protected override void OnHide()
        {
            base.OnHide();

            inputAgent.StopListening();
            inputAgent.Jump.OnPress -= Interact;
        }


        private void Interact()
        {
            if (visible)
            {
                OnInteract?.Invoke();
                Hide();
            }
        }

        private void OnDestroy()
        {
            //GameInput.OnInteract -= Interact;
        }
    }
}
