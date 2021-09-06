﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace SceneM
{
    internal static class SceneDataRepository
    {
        private static Dictionary<int, SceneData> SceneData = new Dictionary<int, SceneData>();

        public static void SaveData(Scene scene)
        {
            IDataPersister[] persisters = GetDataPersisters(scene);

            SceneData sceneData = SceneDataRepository.GetSceneData(scene);

            foreach (IDataPersister persister in persisters)
            {
                DataSettings key = persister.GetDataSettings();
                sceneData.Store(key.dataKey, persister.SaveData(), key.persistenceType);
            }
        }

        public static void LoadData(Scene scene)
        {
            IDataPersister[] persisters = GetDataPersisters(scene);

            SceneData sceneData = SceneDataRepository.GetSceneData(scene);

            foreach (var data in sceneData.SavedData)
            {
                IDataPersister persister = persisters.FirstOrDefault(p => Equals(data.Key, p.GetDataSettings().dataKey));
                if (persister == null)
                    continue;

                persister.LoadData(data.Value.Data);
            }
        }

        public static void DeleteAllLevelData(int[] scenes)
        {
            foreach(int scene in scenes)
            {
                SceneData[scene].DeleteLevelData();
            }
        }

        public static void SaveGlobalSceneData(int buildIndex,string key,Data data,PersistenceType persistenceType)
        {
            SceneData scenedata = GetSceneData(buildIndex);
            scenedata.Store(key, data, persistenceType);
        }

        public static bool TryLoadGlobalSceneData(int buildIndex,string key,out Data data)
        {
            if(SceneData.TryGetValue(buildIndex,out SceneData sceneData))
            {
                return sceneData.TryGetData(key, out data);
            }
            data = new Data();
            return false;
        }

        private static IDataPersister[] GetDataPersisters(Scene scene)
        {
            return SceneUtil.FindAllOfTypeInScene<IDataPersister>(scene).ToArray();
        }

        private static SceneData GetSceneData(Scene scene) => GetSceneData(scene.buildIndex);

        private static SceneData GetSceneData(int buildIndex)
        {
            if (SceneData.TryGetValue(buildIndex, out SceneData sceneData))
            {
                return sceneData;
            }
            else
            {
                SceneData newSceneData = new SceneData();
                SceneData.Add(buildIndex, newSceneData);
                return newSceneData;
            }
        }

        private static bool TryGetSceneData(Scene scene, out SceneData sceneData)
        {
            return SceneData.TryGetValue(scene.buildIndex, out sceneData);
        }
    }

    internal class SceneData
    {
        public Dictionary<string, SavedData> SavedData = new Dictionary<string, SavedData>();

        public void Store(string key,Data data,PersistenceType persistenceType)
        {
            SavedData[key] = new SavedData(data, persistenceType);
        }

        public void DeleteLevelData()
        {
            List<string> removeData = new List<string>();
            foreach(var d in SavedData)
            {
                if(d.Value.PersistenceType == PersistenceType.LevelOnly)
                {
                    removeData.Add(d.Key);
                }
            }

            foreach(string r in removeData)
            {
                SavedData.Remove(r);
            }
        }

        public bool TryGetData(string key,out Data data)
        {
            if(SavedData.TryGetValue(key,out SavedData savedData))
            {
                data = savedData.Data;
                return true;
            }
            data = new Data();
            return false;
        }
    }

    internal class SavedData
    {
        public Data Data;

        public PersistenceType PersistenceType;

        public SavedData(Data data, PersistenceType persistenceType)
        {
            Data = data;
            PersistenceType = persistenceType;
        }
    }

    public enum PersistenceType
    {
        LevelOnly,
        Global
    }
}
