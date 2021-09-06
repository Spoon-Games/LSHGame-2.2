using SceneM;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.UI
{
    [RequireComponent(typeof(Button))]
    public class UILevelSelectComponent : MonoBehaviour
    {
        [SerializeField] private MainSceneInfo sceneInfo;

        [SerializeField] private GameObject indicatorNotStarted;
        [SerializeField] private GameObject indicatorNotCompleted;
        [SerializeField] private GameObject indicatorCompleted;


        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonPress);
        }

        private void OnButtonPress()
        {
            LevelManager.LoadScene(sceneInfo);
        }

        private void SetIndicator()
        {
            var status = SceneMarker.GetSceneStatus(sceneInfo);

            indicatorNotStarted.SetActive(false);
            indicatorNotCompleted.SetActive(false);
            indicatorCompleted.SetActive(false);

            switch (status)
            {
                case SceneStatus.NotBegun:
                    indicatorNotStarted.SetActive(true);
                    break;
                case SceneStatus.Begun:
                    indicatorNotCompleted.SetActive(true);
                    break;
                case SceneStatus.Finished:
                    indicatorCompleted.SetActive(true);
                    break;
            }
        }

        private void OnEnable()
        {
            SetIndicator();
        }


    }
}
