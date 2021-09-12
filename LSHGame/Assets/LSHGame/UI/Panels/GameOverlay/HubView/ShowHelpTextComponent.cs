using UnityEngine;

namespace LSHGame.UI
{
    public class ShowHelpTextComponent : MonoBehaviour
    {
        [SerializeField]
        [Multiline]
        private string text;

        protected bool visible = false;

        public void Show()
        {
            if (!visible)
            {
                HelpTextView.Instance.SetHelpText(text);
                visible = true;

                OnShow();
            }
        }

        protected virtual void OnShow() { }

        public void Hide()
        {
            if (visible)
            {
                HelpTextView.Instance.HideHelpText();
                visible = false;

                OnHide();
            }
        }

        protected virtual void OnHide() { }

        private void OnDestroy()
        {
            if (visible)
            {
                HelpTextView.Instance?.HideHelpText();
                OnHide();
            }
        }
    } 
}
