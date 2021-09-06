using UINavigation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LSHGame.UI
{
    public class SelectLastOnEnterComponent : MonoBehaviour, IPanelTransition
    {
        public GameObject selectOnEnter;
        private GameObject lastSelected;


        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != lastSelected)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
                if (lastSelected != null && !lastSelected.TryGetComponent<IgnorSelectLastOnEnter>(out IgnorSelectLastOnEnter o))
                {
                    selectOnEnter = lastSelected;
                }
            }
        }

        public void Enter(Panel previousPanel) { }

        public void Leave(Panel nextPanel) { }

        public float StartEntering(Panel previousPanel)
        {
            if (selectOnEnter != null)
                EventSystem.current.SetSelectedGameObject(selectOnEnter);

            return 0;
        }

        public float StartLeaving(Panel nextPanel) => 0;
    }
}
