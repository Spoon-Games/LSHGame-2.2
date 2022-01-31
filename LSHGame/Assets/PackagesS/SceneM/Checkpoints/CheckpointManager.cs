using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneM
{
    public static class CheckpointManager
    {
        private static Vector3 currentCheckPos = Vector3.negativeInfinity;

        private static Dictionary<CheckpointInfo, BaseCheckpoint> startCheckpoints = new Dictionary<CheckpointInfo, BaseCheckpoint>();

        private static int _currentOrder = 0;
        public static int CurrentOrder => _currentOrder;

        #region Init
        static CheckpointManager()
        {
            LevelManager.OnStartLoadingMainScene += Reset;
        }

        internal static void Reset(Func<float> o, TransitionInfo transition)
        {
            currentCheckPos = Vector3.negativeInfinity;
            _currentOrder = 0;
        }

        internal static void SetDefaultStartCheckpoint(BaseCheckpoint checkpoint)
        {
            if (!Equals(currentCheckPos, Vector3.negativeInfinity))
                Debug.LogError("Multiple default start checkpoints in the scene. This will probably result in " +
                    "unexpected behaviour.");
            SetCheckpoint(checkpoint);

#if UNITY_EDITOR
            //Debug.Log("IsTempCheckpoint: " + Editor.TempCheckpointEditor.IsTempCheckpoint + " Pos: "+Editor.TempCheckpointEditor.TempCheckpoint);
            if (SceneM.Editor.TempCheckpointEditor.IsTempCheckpoint)
                currentCheckPos = SceneM.Editor.TempCheckpointEditor.TempCheckpoint;
#endif
        }

        internal static void RegisterStartCheckpoint(BaseCheckpoint checkpoint, CheckpointInfo identifier)
        {
            startCheckpoints[identifier] = checkpoint;
        }

        public static void SetStartCheckpoint(CheckpointInfo identifier, bool clearStartCheckpoints = true)
        {
            if (startCheckpoints.TryGetValue(identifier, out BaseCheckpoint checkpoint))
            {
                SetCheckpoint(checkpoint);
            }
            else
                Debug.LogError("The checkpoint with the identifier " + identifier.name + " was not found");
            if (clearStartCheckpoints)
                startCheckpoints.Clear();
        } 
        #endregion

        public static Vector3 GetCheckpointPos()
        {
            if (Equals(currentCheckPos, Vector3.negativeInfinity))
                throw new SceneMException("No start checkpoint was assigned. Please change that!");
            return currentCheckPos;
        }

        internal static bool SetCheckpoint(BaseCheckpoint checkpoint)
        {
            if(checkpoint.Order >= CurrentOrder)
            {
                currentCheckPos = checkpoint.SetCheckpoint();
                _currentOrder = checkpoint.Order;
                return true;
            }
            return false;
        }
    } 
}
