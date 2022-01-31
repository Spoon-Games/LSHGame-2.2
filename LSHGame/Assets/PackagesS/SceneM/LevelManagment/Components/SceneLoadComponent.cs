using UnityEngine;

namespace SceneM
{
    public class SceneLoadComponent : MonoBehaviour
    {
        [SerializeField]
        private MySceneInfo sceneInfo;

        private bool isLoading = false;

        public void LoadScene()
        {
            if (!isLoading)
            {
                LevelManager.LoadScene(sceneInfo);
                isLoading = true;
            }
        }

        public void UnloadScene()
        {
            LevelManager.UnloadScene(sceneInfo); 
        } 
    }
}
