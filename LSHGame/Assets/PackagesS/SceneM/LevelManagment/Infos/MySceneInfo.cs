using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneM
{
    public class MySceneInfo : ScriptableObject {

        [HideInInspector]
        public string ScenePath;

#if UNITY_EDITOR
        public SceneAsset sceneAsset;

        private void OnValidate()
        {
            ScenePath = AssetDatabase.GetAssetPath(sceneAsset);
        }
#endif

        public int GetBuildIndex()
        {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(ScenePath);
            if (buildIndex == -1)
                throw new ArgumentException("The scene of sceneinfo: " + name + " does not exist");

            return buildIndex;
        }
    }

}
