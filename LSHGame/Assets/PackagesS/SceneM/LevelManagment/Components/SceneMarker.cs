using UnityEngine;

namespace SceneM
{
    public enum SceneStatus { NotBegun, Begun, Finished }

    public class SceneMarker : MonoBehaviour
    {
        public readonly static string SCENE_MARKER_KEY = "MarkedSceneStatus";

        public SceneStatus sceneMarkStatus;
        public bool markOnStart = false;


        private void Start()
        {
            if (markOnStart)
                MarkCurrentScene();
        }

        public void MarkCurrentScene() => MarkCurrentScene(sceneMarkStatus);
        public static void MarkCurrentScene(SceneStatus status)
        {
            MarkScene(LevelManager.CurrantScene, status);
        }

        public static void MarkScene(MySceneInfo scene, SceneStatus status) => MarkScene(scene.GetBuildIndex(), status);
        public static void MarkScene(int buildIndex,SceneStatus status)
        {
            var previousStatus = GetSceneStatus(buildIndex);
            if (((int)status) > ((int)previousStatus))
            {
                MarkSceneOverride(buildIndex, status);
            }
        }

        public static void MarkSceneOverride(MySceneInfo scene, SceneStatus status) => MarkSceneOverride(scene.GetBuildIndex(), status);
        public static void MarkSceneOverride(int buildIndex,SceneStatus status)
        {
            GlobalDataRepository.SaveData(buildIndex, SCENE_MARKER_KEY, new Data<SceneStatus>(status));
        }

        public static SceneStatus GetCurrentSceneStatus() => GetSceneStatus(LevelManager.CurrantScene);
        public static SceneStatus GetSceneStatus(MySceneInfo scene) => GetSceneStatus(scene.GetBuildIndex());
        public static SceneStatus GetSceneStatus(int buildIndex)
        {
            if(GlobalDataRepository.TryLoadData(buildIndex, SCENE_MARKER_KEY,out Data data))
            {
                if (data is Data<SceneStatus> d)
                {
                    return d.value;
                }
            }
            return SceneStatus.NotBegun;
        }
    }
}
