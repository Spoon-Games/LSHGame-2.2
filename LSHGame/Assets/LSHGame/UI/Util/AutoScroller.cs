using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Scrollbar))]
    public class AutoScroller : MonoBehaviour, ISelectHandler
    {
        public float durration = 10;
        public float startValue = 1;
        public float endValue = 0;

        private Scrollbar scrollbar;
        private Tween scrollTween;

        private void Start()
        {
            scrollbar = GetComponent<Scrollbar>();

            scrollbar.DOKill(false);
            scrollbar.value = startValue;

            scrollTween = DOTween.To(() => scrollbar.value, (float v) => scrollbar.value = v, endValue, durration).SetEase(Ease.Linear);
        }

        public void OnSelect(BaseEventData eventData)
        {
            scrollTween?.Kill(false);
        }
    }
}
