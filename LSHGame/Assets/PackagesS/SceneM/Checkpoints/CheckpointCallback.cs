using UnityEngine;
using UnityEngine.Events;

namespace SceneM
{
    public class CheckpointCallback : BaseCheckpoint
    {
        public UnityEvent OnActivateCheckpoint;

        public override Vector3 SetCheckpoint()
        {
            Debug.Log("OnActivateCheckpoint " + name);
            OnActivateCheckpoint.Invoke();

            return base.SetCheckpoint();
        }
    }
}
