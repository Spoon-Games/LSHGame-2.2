using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public class RecreateModule : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If the checkpointOrder exceeds this value, the module will not be recreated")]
        private int maxCheckpointOrder = -1;
        public int MaxCheckpointOrder => maxCheckpointOrder;

        private IRecreatable[] recreatables;
        private IRecreateBlocker[] blockers;

        private bool wasDestroied = false;

        private void Awake()
        {
            LevelManager.OnExitScene += OnExitScene;
            LevelManager.OnResetLevel += Recreate;

            recreatables = GetComponentsInChildren<IRecreatable>();
            blockers = GetComponentsInChildren<IRecreateBlocker>();

            if(recreatables.Length == 0)
            {
                Debug.LogWarning("Recreatemodule " + name + " has no Recreateables assigned");
            }
        }

        public void Recreate()
        {
            bool isInOrder = maxCheckpointOrder < 0 || maxCheckpointOrder >= CheckpointManager.CurrentOrder;
            //if(maxCheckpointOrder >= 0)
            //    Debug.Log("CurrentCheckpointOrder: " + CheckpointManager.CurrentOrder + " MaxO: "+maxCheckpointOrder+" IsO: "+isInOrder);

            if (isInOrder && !wasDestroied)
            {
                foreach(var blocker in blockers)
                {
                    if (!blocker.DoesRecreate())
                        return;
                }

                foreach(var recreatable in recreatables)
                {
                    recreatable.Recreate();
                }
            }

            //if (!wasReset && isInOrder)
            //{
            //    if (!wasDestroied)
            //    {
            //        Destroy(gameObject);
            //    }
            //    Deregister();

            //    RecreateModule newModule = RecreateManager.Instance.Recreate(this, position, rotation,scale,parent);
            //    if (newModule != null)
            //    {
            //        newModule.maxCheckpointOrder = maxCheckpointOrder;
            //    }

            //    wasReset = true;
            //}
        }

        private void OnExitScene()
        {
            Deregister();
        }

        private void Deregister()
        {
            LevelManager.OnResetLevel -= Recreate;
            LevelManager.OnExitScene -= OnExitScene;
        }

        private void OnDestroy()
        {
            Deregister();
            wasDestroied = true;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (maxCheckpointOrder > -1)
            {
                UnityEditor.Handles.Label(transform.position,
                    new GUIContent() { text = maxCheckpointOrder.ToString() },
                    new GUIStyle() { contentOffset = new Vector2(-14, -40), fontSize = 20 });
            }
        }

#endif
    }
}
