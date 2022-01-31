using AudioP;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public class PictureView : BaseCharacterView<PictureLog,PictureView>
    {
        [SerializeField]
        private TMP_Text dialogField;
        [SerializeField]
        private Image imageField; 

        [SerializeField]
        private float minScale = 0.3f;
        [SerializeField]
        private float inScaleTime = 0.7f;
        [SerializeField]
        private Ease inScaleEase = Ease.OutQuad;

        [SerializeField]
        private float outScaleTime = 0.5f;
        [SerializeField]
        private Ease outScaleEase = Ease.InQuad;

        [SerializeField] private SoundInfo defaultOpeningSound;

        protected override float OnStartEntering()
        {
            base.OnStartEntering();

            imageField.sprite = Dialog.Picture;
            imageField.rectTransform.localScale = new Vector3(minScale, minScale, 1);
            imageField.rectTransform.DOScale(Vector3.one, inScaleTime).SetEase(inScaleEase);

            dialogField.text = "";

            AudioManager.Play(Dialog.OpeningSound??defaultOpeningSound);

            return inScaleTime;
        }

        #region Text

        protected override void SetText(string text)
        {
            dialogField.text = text;
            //Debug.Log("PictureText: " + text);
        }
        #endregion

        protected override float OnStartLeaving()
        {
            base.OnStartLeaving();

            imageField.rectTransform.DOScale(new Vector3(minScale, minScale, 1), outScaleTime).SetEase(outScaleEase);
            return outScaleTime;
        }
    }
}
