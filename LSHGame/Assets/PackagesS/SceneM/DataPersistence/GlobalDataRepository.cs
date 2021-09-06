using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneM
{
    public static class GlobalDataRepository
    {
        public static void SaveData(string key, Data data, PersistenceType persistenceType = PersistenceType.Global)
        {
            SaveData(LevelManager.CurrantScene, key, data, persistenceType);
        }

        public static void SaveData(MySceneInfo scene, string key, Data data, PersistenceType persistenceType = PersistenceType.Global)
        {
            SaveData(scene.GetBuildIndex(), key, data, persistenceType);

        }

        public static void SaveData(int buildIndex, string key, Data data, PersistenceType persistenceType = PersistenceType.Global)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key can not be null or empty");

            if (buildIndex == -1)
                throw new ArgumentException("The build index can not be -1");

            SceneDataRepository.SaveGlobalSceneData(buildIndex, key, data, persistenceType);
        }

        public static bool TryLoadData(string key, out Data data)
        {
            return TryLoadData(LevelManager.CurrantScene, key, out data);
        }

        public static bool TryLoadData(MySceneInfo scene, string key, out Data data)
        {            
            if(scene == null)
            {
                Debug.LogError("The sceneInfo: " + scene + " is null");
                data = new Data();
                return false;
            }

            return TryLoadData(scene.GetBuildIndex(), key, out data);

        }

        public static bool TryLoadData(int buildIndex, string key, out Data data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key can not be null or empty");

            if (buildIndex == -1)
                throw new ArgumentException("The build index can not be -1");

            return SceneDataRepository.TryLoadGlobalSceneData(buildIndex, key, out data);
        }


    }
}
