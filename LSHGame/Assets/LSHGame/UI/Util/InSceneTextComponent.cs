using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    public class InSceneTextComponent : MonoBehaviour
    {
        [SerializeField] private TMP_Text textField;
        [SerializeField] private SpriteRenderer background;

        [SerializeField] private float fadeDurration = 0.1f;

        private bool _visible;
        public bool Visible { get => _visible; set
            {
                if(value != _visible)
                {
                    if (value)
                        Show();
                    else
                        Hide();
                }
            } }

        private void Awake()
        {
            gameObject.SetActive(false);
            textField.alpha = 0;
            var c = background.color;
            c.a = 0;
            background.color = c;
        }

        public void Show()
        {
            if (Visible)
                return;
            _visible = true;

            gameObject.SetActive(true);
            textField.DOKill();
            background.DOKill();

            textField.DOFade(1, fadeDurration).SetEase(Ease.OutQuad);
            background.DOFade(1, fadeDurration).SetEase(Ease.OutQuad);

        }

        public void Hide()
        {
            if (!Visible)
                return;
            _visible = false;

            textField.DOKill();
            background.DOKill();

            textField.DOFade(0, fadeDurration).SetEase(Ease.InQuad).OnComplete(() => gameObject.SetActive(false));
            background.DOFade(0, fadeDurration).SetEase(Ease.InQuad);
        }
    }
}
