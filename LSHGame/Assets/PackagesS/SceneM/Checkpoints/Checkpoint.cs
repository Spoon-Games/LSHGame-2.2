using UnityEngine;

namespace SceneM
{
    public class Checkpoint : BaseCheckpoint
    {
        

        public enum CheckType { Stay, Vanish }

        public CheckType checkType;

        [SerializeField]
        private bool autoPrioritize = true;


        public override Vector3 SetCheckpoint()
        {
            if (checkType == CheckType.Vanish)
                Destroy(gameObject);

            return base.SetCheckpoint();
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoSetOrder();
        }

        private void OnDrawGizmos()
        {
            AutoSetOrder();

            if (isDefaultStartCheckpoint)
                Gizmos.DrawIcon(transform.position, "checkpoint-default-start", true);
            else if (identifier != null)
                Gizmos.DrawIcon(transform.position, "checkpoint-start", true);
            else
                Gizmos.DrawIcon(transform.position, "checkpoint", true);

            UnityEditor.Handles.Label(transform.position,
                new GUIContent() { text = order.ToString() },
                new GUIStyle() { contentOffset = new Vector2(-14, -40), fontSize = 20 });
        }

        private void AutoSetOrder()
        {
            if (autoPrioritize && Application.isEditor && !UnityEditor.EditorApplication.isPlaying)
            {
                order = transform.GetSiblingIndex();
                isDefaultStartCheckpoint = order == 0;
                
            }
        }
#endif
    }

    public abstract class BaseCheckpoint : MonoBehaviour
    {
        [SerializeField]
        public bool isDefaultStartCheckpoint;

        [SerializeField]
        protected int order = 0;
        internal int Order => order;

        [SerializeField]
        protected CheckpointInfo identifier;

        protected virtual void Awake()
        {
            if (isDefaultStartCheckpoint)
                CheckpointManager.SetDefaultStartCheckpoint(this);
            if (identifier != null)
                CheckpointManager.RegisterStartCheckpoint(this, identifier);
        }

        public void TriggerCheckpoint()
        {
            CheckpointManager.SetCheckpoint(this);
        }

        public virtual Vector3 SetCheckpoint()
        {
            return transform.position;
        }
    }
}
