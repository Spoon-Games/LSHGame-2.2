﻿

using SceneM;
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental.SceneManagement;
#endif

using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(menuName ="LSHGame/Editor/RecreateManager")]
    public class RecreateManager : SceneM.ScriptableSingleton<RecreateManager>
    {
        [SerializeField]
        private RecreateModule[] serializedModules;   

        public RecreateModule Recreate(RecreateModule ghost, Vector3 originPosition, Quaternion originRotation,Vector3 originScale,Transform originParent)
        {
            var vessel = serializedModules.FirstOrDefault(m => Equals(m.prefabGuid,ghost.prefabGuid)); // Maybe make it more specific
            if(vessel != null)
            {
                RecreateModule o = Instantiate(vessel, originPosition, originRotation, originParent);
                o.SetLocalScale(originScale);
                return o;
            }
            else
            {
                Debug.Log("Recreate prefab was not found for: " + ghost.name + " guid: " + ghost.prefabGuid);
            }
            return null;
        }

#if UNITY_EDITOR

        public void LoadPrefabs()
        {
            List<RecreateModule> modules = new List<RecreateModule>();
            //var paths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(Substance).ToString());
            //foreach (var path in paths)
            //{
            //    substances.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<Substance>(path));
            //}

            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                 
                RecreateModule m = AssetDatabase.LoadAssetAtPath<RecreateModule>(path);
                if (m != null)
                {
                    if (string.IsNullOrEmpty(m.prefabGuid))
                        m.prefabGuid = Guid.NewGuid().ToString();
                    modules.Add(m);
                    //Debug.Log("Path: " + path + " GUID: " + guid);
                }
            }
            serializedModules = modules.ToArray();

            //AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private void OnEnable()
        {
            PrefabStage.prefabSaved += OnPrefabSaved;
        }

        private void OnPrefabSaved(GameObject prefab)
        {
            if(prefab != null && prefab.TryGetComponent<RecreateModule>(out RecreateModule module) && string.IsNullOrEmpty(module.prefabGuid))
            {
                //module.prefabPath = Guid.NewGuid().ToString();
                var so = new SerializedObject(module);
                so.FindProperty("prefabGuid").stringValue = Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
                Debug.Log("Saved Prefab "+module.prefabGuid);

                LoadPrefabs();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [MenuItem("Assets/Load Prefabs")]
        private static void OnRuntimeInitialize()
        {
            Instance.LoadPrefabs();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Instance.LoadPrefabs();
        }


#endif
    }
}
