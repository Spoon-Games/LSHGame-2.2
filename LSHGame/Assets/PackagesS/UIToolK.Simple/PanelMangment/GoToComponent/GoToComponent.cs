using UnityEngine;
using UnityEngine.UI;

namespace UINavigation
{
    public class GoToComponent : BaseGoToComponent
    {
        [SerializeField]
        public Panel nextPanel;

        public override void GoToNext()
        {
            if (manager == null)
                return;

            manager.ShowPanelByP(nextPanel);
        }
    }

    public abstract class BaseGoToComponent : MonoBehaviour
    {
        public PanelManager manager;

        protected virtual void Awake()
        {
            if(manager == null)
                manager = GetComponentInParent<PanelManager>();
            if (manager == null)
            {
                Debug.LogError("No IGoToNextManager was found in parents GoToNextComponent: " + name);
            }
            if(TryGetComponent<Button>(out Button button))
            {
                button.onClick.AddListener(GoToNext);
            }
        }

        public abstract void GoToNext();
    }
}
