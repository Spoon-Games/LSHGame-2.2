using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class LinearSpriteSheetMapper : LinearMapper
    {
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite[] sprites;

        protected override int ArrayLength { get => sprites.Length; }

        protected override bool UpdateEveryFrame => true; 

        private SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = animator.GetComponent<SpriteRenderer>();
                return _spriteRenderer;
            }
        }

        protected override void SetIndex(int index)
        {
            SpriteRenderer.sprite = sprites[index];
        }
    }
}
