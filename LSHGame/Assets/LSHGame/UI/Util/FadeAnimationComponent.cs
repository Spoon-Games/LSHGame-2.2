using DG.Tweening;
using UINavigation;
using UnityEngine;

namespace LSHGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimationComponent : MonoBehaviour, IPanelTransition
    {
        public float FadeInTime = 0.5f;
        public Ease FadeInEase = Ease.OutQuad;
        [Space]
        public float FadeOutTime = 0.5f;
        public Ease FadeOutEase = Ease.InQuad;

        private CanvasGroup _canvasGroup;
        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        public void Enter(Panel previousPanel) { }


        public void Leave(Panel nextPanel) { }



        public float StartEntering(Panel previousPanel)
        {
            CanvasGroup.DOKill();
            CanvasGroup.alpha = 0;
            CanvasGroup.DOFade(1, FadeInTime).SetEase(FadeInEase);
            return FadeInTime;
        }

        public float StartLeaving(Panel nextPanel)
        {
            CanvasGroup.DOKill();
            CanvasGroup.DOFade(0, FadeOutTime).SetEase(FadeOutEase);
            return FadeOutTime;
        }
    }
}
