using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InputC.Editor
{
    [CustomEditor(typeof(InputAgent),true)]
    public class InputAgentEditor : UnityEditor.Editor
    {
        private InputAgent agent;

        private void OnEnable()
        {
            agent = target as InputAgent;
        }

        public override void OnInspectorGUI()
        {
            List<bool> wasRegisteredKeys = new List<bool>();
            foreach(var key in agent.GetKeys())
            {
                wasRegisteredKeys.Add(key.IsRegistered);
            }

            base.OnInspectorGUI();

            bool blockAll = true;


            int i = 0;
            foreach(var key in agent.GetKeys())
            {
                if(wasRegisteredKeys[i] != key.IsRegistered)
                {
                    key.Editor_SetIsBlocking(key.IsRegistered);
                }
                i++;
            }

            foreach(var key in agent.GetKeys())
            {
                blockAll &= key.IsBlocking;
            }

            bool newBlockAll = EditorGUILayout.Toggle("Block all", blockAll);

            if(newBlockAll && !blockAll)
            {
                foreach (var key in agent.GetKeys())
                    key.Editor_SetIsBlocking(true);
            }else if(!newBlockAll && blockAll)
            {
                foreach(var key in agent.GetKeys())
                {
                    if (!key.IsRegistered)
                        key.Editor_SetIsBlocking(false);
                }
            }
        }
    } 
}
