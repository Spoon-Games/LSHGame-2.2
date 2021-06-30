using TMPro;
using UnityEngine;

namespace LSHGame.Util
{
    public class DebugFPSView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text fpsText;

        [SerializeField]
        private float refreshRate = 0.5f;

        [SerializeField]
        private int version = 1;

        private GameObject content => fpsText.gameObject;
        private bool visible => content.activeSelf;

        private string log = "";

        private float timeCounter;
        private float frameCounter;
        private float maxSpike;


        private void Awake()
        {
            GameInput.ToggleDebugFPSView += ToggleVisible;

            SetVisible(false);
        }

        private void Update()
        {
            if (visible)
            {
                maxSpike = Mathf.Max(Time.deltaTime, maxSpike);
                if (timeCounter < refreshRate)
                {
                    timeCounter += Time.deltaTime;
                    frameCounter++;
                }
                else
                {
                    float fps = frameCounter / timeCounter;
                    fpsText.text = string.Format("FPS: {0:N0}\nMax: {1:N0}ms\nV: {2}", fps, maxSpike * 1000,version);

                    timeCounter = 0;
                    frameCounter = 0;
                    maxSpike = 0;
                }
            }
        }


        public void ToggleVisible()
        {
            SetVisible(!content.activeInHierarchy);
        }

        public void SetVisible(bool visible)
        {
            content.SetActive(visible);
        }


        private void OnDestroy()
        {
            GameInput.ToggleDebugFPSView -= ToggleVisible;
        }
    }
}
