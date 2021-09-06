using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Animator))]
    public class SelectionAnimatorComponent : MonoBehaviour,ISelectHandler,IDeselectHandler
    {
        [SerializeField] private string deselectTrigger = "Deselected";
        [SerializeField] private string selectTrigger = "Selected";
        [SerializeField] private string buttonPressedTrigger = "Pressed";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if(TryGetComponent<Button>(out Button b))
            {
                b.onClick.AddListener(() => animator.SetTrigger(buttonPressedTrigger));
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            animator.SetTrigger(deselectTrigger);
        }

        public void OnSelect(BaseEventData eventData)
        {
            animator.ResetTrigger(deselectTrigger);
            animator.SetTrigger(selectTrigger);
        }

        private void OnEnable()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
                OnSelect(null);
        }
    }
}
