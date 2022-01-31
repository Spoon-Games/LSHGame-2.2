using UnityEngine;

namespace LSHGame.UI
{
    public class CursorCaptureComponent : MonoBehaviour
    {
        [SerializeField]
        private bool lockOnEnable = true;

        private void OnEnable()
        {
            if (lockOnEnable)
                SetCursorLocked(true);
        }

        public void SetCursorLocked(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
        }

        private void OnDisable()
        {
            if (lockOnEnable)
                SetCursorLocked(false);
        }
    }
}
