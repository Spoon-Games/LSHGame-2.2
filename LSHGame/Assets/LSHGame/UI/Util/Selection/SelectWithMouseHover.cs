using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Selectable))]
    public class SelectWithMouseHover : MonoBehaviour,IPointerEnterHandler
    {
        private Selectable selectable;

        public void OnPointerEnter(PointerEventData eventData)
        {
            selectable.Select();
        }

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
        }
    }
}
