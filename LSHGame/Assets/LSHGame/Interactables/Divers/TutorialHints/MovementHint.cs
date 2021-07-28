using LSHGame.UI;
using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame
{
    public class MovementHint : MonoBehaviour
    {
        [SerializeField] private InSceneTextComponent text;

        public enum HintType { Movement, LadderClimbing}
        [SerializeField]
        private HintType hintType;

        [SerializeField]
        private float delay;

        [SerializeField]
        private bool hintOnStart = false;

        private bool isActive = false;

        private void Start()
        {
            switch (hintType)
            {
                case HintType.Movement:
                    GameInput.Hint_Movement += ActionExecuted;
                    break;
                case HintType.LadderClimbing:
                    GameInput.Hint_LadderClimb += ActionExecuted;
                    break;
            }

            if (hintOnStart)
                StartHint();
        }

        public void StartHint()
        {
            if (!isActive)
            {
                TimeSystem.Delay(delay, t => { Show(); });
                isActive = true;
            }
        }

        private void Show()
        {
            if (isActive)
            {
                text.Show();
            }
        }


        public void Hide()
        {
            if (isActive)
            {
                text.Hide();
                isActive = false;
            }
        }

        private void ActionExecuted()
        {
            Hide();
        }

        private void OnDestroy()
        {
            GameInput.Hint_Movement -= ActionExecuted;
            GameInput.Hint_LadderClimb -= ActionExecuted;
        }
    }
}