using LSHGame.UI;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame
{
    public class JumpWallHint : MonoBehaviour
    {
        [SerializeField] private InSceneTextComponent jumpFromWall;
        [SerializeField] private InSceneTextComponent jumpAgainstWall;

        private bool isInArea;

        private void Awake()
        {
            GameInput.Hint_WallClimb += OnWallClimb;
        }

        public void Activate()
        {
            if (isInArea)
                return;
            isInArea = true;
            jumpAgainstWall.Show();
        }

        public void Deactivate()
        {
            if (!isInArea)
                return;
            isInArea = false;

            jumpFromWall.Hide();
            jumpAgainstWall.Hide();
        }

        private void OnWallClimb()
        {
            if (isInArea)
            {
                jumpFromWall.Show();
                //jumpAgainstWall.Hide();
            }
        }

        private void OnDestroy()
        {
            GameInput.Hint_WallClimb -= OnWallClimb;
        }
    }
}
