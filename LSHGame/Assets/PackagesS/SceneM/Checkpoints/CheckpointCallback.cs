using UnityEngine;
using UnityEngine.Events;

namespace SceneM
{
    public class CheckpointCallback : BaseCheckpoint
    {
        public UnityEvent OnActivateCheckpoint;

        public override Vector3 SetCheckpoint()
        {
            OnActivateCheckpoint.Invoke();

            return base.SetCheckpoint();
        }
    }
}
