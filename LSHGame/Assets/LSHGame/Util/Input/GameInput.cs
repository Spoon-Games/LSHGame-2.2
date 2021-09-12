using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.Util
{
    public static class GameInput
    {

        #region Debug
        public static Action ToggleDebugConsole;
        public static Action ToggleDebugFPSView;
        public static Action ToggleDebugSceneView;
        #endregion

        #region HintStates
        public static Action Hint_WallClimb;
        public static Action Hint_LadderClimb;
        public static bool Hint_IsLilium = false;
        public static Action Hint_Movement;
        #endregion



        #region Init
        public static InputController Controller { get; private set; }

        static GameInput()
        {
            Controller = new InputController();
            Controller.Enable();

            //Debug
            Controller.Debug.ToggleConsole.performed += ctx => ToggleDebugConsole?.Invoke();
            Controller.Debug.ToggleFPSView.performed += ctx => ToggleDebugFPSView?.Invoke();
            Controller.Debug.ToggleSceneView.performed += ctx => ToggleDebugSceneView?.Invoke();

        } 
        #endregion

#if UNITY_EDITOR
        public static void Debug_OverrideJump(bool performed) => Debug.LogError("Override not implemented");

        public static void Debug_OverrideWASD(bool W,bool A,bool S, bool D)
        {
            Debug.LogError("Override not implemented");
        }
#endif
    } 

}
