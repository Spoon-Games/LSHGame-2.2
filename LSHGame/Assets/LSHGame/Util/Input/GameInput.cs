using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LSHGame.Util
{
    public static class GameInput
    {
        #region Player
        private static Vector2 overrideMovmentInput;
        public static Vector2 MovmentInput => overrideMovmentInput == Vector2.zero? Controller.Player.Movement.ReadValue<Vector2>() : overrideMovmentInput;

        private static InputActionWrapper dashWrapper;
        public static bool IsDash => dashWrapper.IsPerformed;
        public static bool WasDashRealeased => dashWrapper.IsRealeasedThisFrame;

        private static InputActionWrapper jumpWrapper;
        public static bool IsJump => jumpWrapper.IsPerformed;

        private static InputActionWrapper wallClimbHoldWrapper;
        public static bool IsWallClimbHold => Keyboard.current.shiftKey.isPressed;//wallClimbHoldWrapper.IsPerformed;

        public static Action<bool> OnCameraLookDown;

        public static Action OnInteract;
        public static Action OnInteractCancel;
        #endregion

        #region Dialog
        public static Action OnFurther;
        #endregion

        #region Mix
        public static Action Mix_OnUIBack;
        public static Action Mix_OnUIBack_Release;
        #endregion

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

            //Player
            dashWrapper = new InputActionWrapper(Controller.Player.Dash);
            jumpWrapper = new InputActionWrapper(Controller.Player.Jump);
            wallClimbHoldWrapper = new InputActionWrapper(Controller.Player.WallClimbHold);

            Controller.Player.CameraLook.performed += ctx => OnCameraLookDown?.Invoke(true);
            Controller.Player.CameraLook.canceled += ctx => OnCameraLookDown?.Invoke(false);

            Controller.Player.Interact.performed += ctx => OnInteract?.Invoke();
            Controller.Player.Interact.canceled += ctx => OnInteractCancel?.Invoke();

            //UI
            Controller.Dialog.Further.performed += ctx => { OnFurther?.Invoke();};

            //Debug
            Controller.Debug.ToggleConsole.performed += ctx => ToggleDebugConsole?.Invoke();
            Controller.Debug.ToggleFPSView.performed += ctx => ToggleDebugFPSView?.Invoke();
            Controller.Debug.ToggleSceneView.performed += ctx => ToggleDebugSceneView?.Invoke();

            //Mix
            Controller.Player.BackUI.performed += ctx => Mix_OnUIBack?.Invoke();
            Controller.Dialog.Back.performed += ctx => Mix_OnUIBack?.Invoke();
            Controller.UI.Back.performed += ctx => Mix_OnUIBack?.Invoke();

            Controller.Player.BackUI.canceled += ctx => Mix_OnUIBack_Release?.Invoke();
            Controller.Dialog.Back.canceled += ctx => Mix_OnUIBack_Release?.Invoke();
            Controller.UI.Back.canceled += ctx => Mix_OnUIBack_Release?.Invoke();


            UINavigation.Util.SetInputController += SetInputController;
        } 

        public static void SetInputController(string inputController)
        {
            switch (inputController)
            {
                case "Player":EnablePlayer();break;
                case "UI": EnableUI(); break;//EnableMap(Controller.UI.Get()); break;
                case "Dialog": EnableDialog(); break;//EnableMap(Controller.Dialog.Get()); break;
            }
        }

        private static void EnablePlayer()
        {
            Controller.Player.Enable();
            Controller.UI.Disable();
            Controller.Dialog.Disable();
        }

        private static void EnableUI()
        {
            Controller.Player.Disable();
            Controller.UI.Enable();
            Controller.Dialog.Disable();
        }

        private static void EnableDialog()
        {
            Controller.Player.Disable();
            Controller.UI.Disable();
            Controller.Dialog.Enable();
        }

        private static void EnableMap(InputActionMap inputMap)
        {
            foreach (var map in Controller.asset.actionMaps)
                if (map != inputMap || map != Controller.Debug.Get())
                    map.Disable();
            inputMap.Enable();
        }
        #endregion

#if UNITY_EDITOR
        public static void Debug_OverrideJump(bool performed) => jumpWrapper.OverrideInput(performed);

        public static void Debug_OverrideWASD(bool W,bool A,bool S, bool D)
        {
            overrideMovmentInput = Vector2.zero;
            if (W)
                overrideMovmentInput.y = 1;
            if (A)
                overrideMovmentInput.x = -1;
            if (S)
                overrideMovmentInput.y = -1;
            if (D)
                overrideMovmentInput.x = 1;
        }
#endif
    } 

    public class InputActionWrapper
    {
        public readonly InputAction InputAction;

        public bool IsPerformed = false;

        public bool IsRealeasedThisFrame => releaseFrame == Time.frameCount;

        private int releaseFrame = -1;

        public InputActionWrapper(InputAction inputAction)
        {
            InputAction = inputAction;
            inputAction.performed += ctx => IsPerformed = true;
            inputAction.canceled += ctx =>
            {
                IsPerformed = false;
                releaseFrame = Time.frameCount;
            };
        }

        internal void OverrideInput(bool performed)
        {
            IsPerformed = performed;
            if (!performed)
                releaseFrame = Time.frameCount;
        }
    }
}
