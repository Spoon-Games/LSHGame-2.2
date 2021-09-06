using SceneM;
using UnityEngine;

namespace LSHGame.UI
{
    public class HideLevelSelectButtonComponent : MonoBehaviour
    {
        [SerializeField] MainSceneInfo startScene;

        private void Awake()
        {
            var status = SceneMarker.GetSceneStatus(startScene);
            Debug.Log($"StartScene {startScene} Status: {status}");
            if (status == SceneStatus.NotBegun)
                gameObject.SetActive(false);
        }
    }
}
