using LSHGame.Util;
using UINavigation;
using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.UI
{
    public class UIBackButtonComponent : MonoBehaviour
    {
        public GlobalInputAgent agent;

        public UnityEvent OnBackPress;

        private GoToComponent goToComponent;

        private void Awake()
        {
            TryGetComponent<GoToComponent>(out goToComponent);
        }

        private void OnPress()
        {
            OnBackPress.Invoke();
            if (goToComponent != null)
                goToComponent.GoToNext();
        }

        private void OnEnable()
        {
            agent.Back.OnPress += OnPress;
            agent.Listen();
        }

        private void OnDisable()
        {
            agent.Back.OnPress -= OnPress;
            agent.StopListening();
            
        }

    }
}
